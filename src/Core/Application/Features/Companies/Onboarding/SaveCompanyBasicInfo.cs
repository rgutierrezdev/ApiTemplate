using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class SaveCompanyBasicInfo
{
    public class Request : BaseRequest, IRequest
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest : BaseSaveCompanyBasicInfo.Request
    {
        public CompanyType Type { get; set; }
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty();

            Include(new BaseValidator());
        }
    }

    internal class BaseValidator : AbstractValidator<BaseRequest>
    {
        public BaseValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty();

            Include(new BaseSaveCompanyBasicInfo.Validator());
        }
    }

    internal class Handler : IRequestHandler<Request>
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

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync(query => query
                    .Include(c => c.CompanyChange)
                    .Where(c => c.Id == request.CompanyId),
                cancellationToken
            ) ?? throw new NotFoundException(
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

            change.LegalType = request.LegalType;
            change.LegalName = request.LegalName;
            change.CiiuCode = request.CiiuCode;
            change.PersonType = request.PersonType;
            change.BusinessStructureId = request.BusinessStructureId;
            change.DocumentTypeId = request.DocumentTypeId;
            change.Document = request.Document;
            change.VerificationDigit = request.VerificationDigit;
            change.CityId = request.CityId;
            change.Address = request.Address;

            if (change.PersonType == PersonType.Legal)
            {
                change.LegalRepresentativeFirstName = request.LegalRepresentative!.FirstName;
                change.LegalRepresentativeLastName = request.LegalRepresentative.LastName;
                change.LegalRepresentativeEmail = request.LegalRepresentative.Email;
                change.LegalRepresentativeDocumentTypeId = request.LegalRepresentative.DocumentTypeId;
                change.LegalRepresentativeDocument = request.LegalRepresentative.Document;
            }
            else
            {
                change.LegalRepresentativeFirstName = null;
                change.LegalRepresentativeLastName = null;
                change.LegalRepresentativeEmail = null;
                change.LegalRepresentativeDocumentTypeId = null;
                change.LegalRepresentativeDocument = null;
            }

            company.Type = request.Type;
            company.SetLastOnboardingStep(OnboardingStep.LegalNotices);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
