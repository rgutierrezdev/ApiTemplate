using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class SaveCompanyAssociates
{
    public class Request : BaseRequest, IRequest<List<Guid>>
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest : BaseSaveCompanyAssociates.Request
    {
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.CompanyId)
                .NotEmpty();

            Include(new BaseValidator());
        }
    }

    internal class BaseValidator : AbstractValidator<BaseRequest>
    {
        public BaseValidator()
        {
            Include(new BaseSaveCompanyAssociates.Validator());
        }
    }

    internal class Handler : IRequestHandler<Request, List<Guid>>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<List<Guid>> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync(query => query
                                  .Include(c => c.CompanyChange)
                                  .Include(c => c.CompanyAssociates.Where(ca => ca.IsChange))
                                  .Where(c => c.Id == request.CompanyId),
                              cancellationToken
                          ) ??
                          throw new NotFoundException(
                              ErrorCodes.CompanyNotFound,
                              $"Company with id '{request.CompanyId}' was not found"
                          );

            if (company.Status != CompanyStatus.OnBoarding)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyInvalidStatus,
                    $"This company status is '{company.Status.ToString()}'"
                );
            }

            var change = company.InitChange();

            change.HasPepRelative = request.HasPepRelative;
            change.UnderOath = request.UnderOath;

            var associateIds = new List<Guid>();

            foreach (var associate in request.Associates)
            {
                CompanyAssociate companyAssociate;

                if (associate.Id == null)
                {
                    companyAssociate = new CompanyAssociate()
                    {
                        Id = Ulid.NewGuid(),
                        CompanyId = company.Id,
                        IsChange = true,
                    };

                    company.CompanyAssociates.Add(companyAssociate);
                }
                else
                {
                    companyAssociate = company.CompanyAssociates.First(ca => ca.Id == associate.Id);
                }

                companyAssociate.Name = associate.Name;
                companyAssociate.DocumentTypeId = associate.DocumentTypeId;
                companyAssociate.Document = associate.Document;
                companyAssociate.ParticipationPercent = associate.ParticipationPercent;
                companyAssociate.Pep = associate.Pep;

                associateIds.Add(companyAssociate.Id);
            }

            foreach (var associateId in request.RemovedAssociateIds)
            {
                var companyAssociate = company.CompanyAssociates
                    .FirstOrDefault(ca => ca.Id == associateId && ca.IsChange);

                if (companyAssociate == null) continue;

                company.CompanyAssociates.Remove(companyAssociate);
            }

            company.SetLastOnboardingStep(OnboardingStep.Summary);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return associateIds;
        }
    }
}
