using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class SaveCompanyProfileAssociates
{
    public class Request : BaseRequest, IRequest
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

    internal class Handler : IRequestHandler<Request>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IUtilsService _utilsService;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            IUtilsService utilsService
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _utilsService = utilsService;
        }

        private static string[] AssociateProps => new[]
        {
            nameof(CompanyAssociate.Name),
            nameof(CompanyAssociate.DocumentTypeId),
            nameof(CompanyAssociate.Document),
            nameof(CompanyAssociate.ParticipationPercent),
            nameof(CompanyAssociate.Pep),
        };

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync(query => query
                    .Include(c => c.CompanyChange)
                    .Include(c => c.CompanyAssociates)
                    .Where(c => c.Id == request.CompanyId),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.CompanyId}' was not found"
            );

            if (company.Status == CompanyStatus.Reviewing ||
                company.CompanyChange?.AssociatesReviewStatus == ReviewStatus.Reviewing)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyInReview,
                    $"This Company status is {ReviewStatus.Reviewing.ToString()}"
                );
            }

            var currentAssociates = company.CompanyAssociates.Where(ca => !ca.IsChange).ToList();
            var tempAssociates = GenerateTempAssociates(request, company);
            var hasSameAssociates = CompareAssociates(currentAssociates, tempAssociates);

            var props = new[]
            {
                nameof(CompanyChange.HasPepRelative),
                nameof(CompanyChange.UnderOath),
            };

            var temp = new CompanyChange
            {
                HasPepRelative = request.HasPepRelative,
                UnderOath = request.UnderOath
            };

            var change = company.InitChange();

            var hasSameValues = _utilsService.CompareProperties(company, temp, props);
            if (hasSameValues && hasSameAssociates)
            {
                _utilsService.AssignNullValues(change, props);

                var changeAssociates = company.CompanyAssociates.Where(ca => ca.IsChange);

                foreach (var changeAssociate in changeAssociates)
                {
                    company.CompanyAssociates.Remove(changeAssociate);
                }

                change.AssociatesReviewStatus = null;
            }
            else
            {
                _utilsService.AssignProperties(temp, change, props);

                foreach (var tempAssociate in tempAssociates)
                {
                    var currentAssociate = company.CompanyAssociates.FirstOrDefault(ca => ca.Id == tempAssociate.Id);

                    if (currentAssociate == null)
                    {
                        company.CompanyAssociates.Add(tempAssociate);
                    }
                    else
                    {
                        _utilsService.AssignProperties(tempAssociate, currentAssociate, AssociateProps);
                    }
                }

                foreach (var associateId in request.RemovedAssociateIds)
                {
                    var companyAssociate = tempAssociates.FirstOrDefault(ca => ca.Id == associateId);

                    if (companyAssociate == null) continue;

                    company.CompanyAssociates.Remove(companyAssociate);
                }

                change.AssociatesReviewStatus = ReviewStatus.ReadyToReview;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        private bool CompareAssociates(List<CompanyAssociate> currentAssociates, List<CompanyAssociate> tempAssociates)
        {
            var hasSameAssociatesCount = currentAssociates.Count == tempAssociates.Count;
            if (!hasSameAssociatesCount)
                return false;

            currentAssociates.Sort((ca1, ca2) => string.Compare(ca1.Name, ca2.Name, StringComparison.Ordinal));
            tempAssociates.Sort((ca1, ca2) => string.Compare(ca1.Name, ca2.Name, StringComparison.Ordinal));

            for (var i = 0; i < currentAssociates.Count; i++)
            {
                var currentAssociate = currentAssociates[i];
                var tempAssociate = tempAssociates[i];

                var hasSameAssociateValues = _utilsService.CompareProperties(
                    currentAssociate,
                    tempAssociate,
                    AssociateProps
                );

                if (hasSameAssociateValues) continue;

                return false;
            }

            return true;
        }

        private static List<CompanyAssociate> GenerateTempAssociates(Request request, Company company)
        {
            var tempAssociates = company.CompanyAssociates.Where(ca => ca.IsChange).ToList();

            foreach (var associate in request.Associates)
            {
                CompanyAssociate companyAssociate;

                if (associate.Id == null)
                {
                    companyAssociate = new CompanyAssociate()
                    {
                        Id = Ulid.NewGuid(),
                        CompanyId = request.CompanyId,
                        IsChange = true,
                    };
                }
                else
                {
                    var existing = tempAssociates.First(ca => ca.Id == associate.Id);
                    tempAssociates.Remove(existing);

                    companyAssociate = new CompanyAssociate()
                    {
                        Id = existing.Id,
                        CompanyId = existing.CompanyId,
                        IsChange = existing.IsChange,
                    };
                }

                companyAssociate.Name = associate.Name;
                companyAssociate.DocumentTypeId = associate.DocumentTypeId;
                companyAssociate.Document = associate.Document;
                companyAssociate.ParticipationPercent = associate.ParticipationPercent;
                companyAssociate.Pep = associate.Pep;

                tempAssociates.Add(companyAssociate);
            }

            foreach (var associateId in request.RemovedAssociateIds)
            {
                var companyAssociate = tempAssociates.FirstOrDefault(ca => ca.Id == associateId);

                if (companyAssociate == null) continue;

                tempAssociates.Remove(companyAssociate);
            }

            return tempAssociates;
        }
    }
}
