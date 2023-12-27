namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class SaveCompanyPaymentType
{
    public class Request : BaseRequest, IRequest
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest
    {
        public bool CreditEnabled { get; set; }
        public bool AuthorizesFinancialInformation { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.CompanyId)
                .NotEmpty();

            Include(new BaseValidator());
        }
    }

    public class BaseValidator : AbstractValidator<BaseRequest>
    {
        public BaseValidator()
        {
            RuleFor(r => r.AuthorizesFinancialInformation)
                .Equal(true)
                .When(r => r.CreditEnabled);
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

            if (request.CreditEnabled)
            {
                change.CreditEnabled = true;
                change.AuthorizesFinancialInformation = request.AuthorizesFinancialInformation;
            }
            else
            {
                change.CreditEnabled = null;
                change.AuthorizesFinancialInformation = null;
            }

            company.SetLastOnboardingStep(OnboardingStep.Documents);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
