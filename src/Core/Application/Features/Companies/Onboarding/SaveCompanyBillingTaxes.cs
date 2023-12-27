using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class SaveCompanyBillingTaxes
{
    public class Request : BaseRequest, IRequest
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest : BaseSaveCompanyBillingTaxes.Request
    {
        public BaseSaveElectronicInvoice.Request? ElectronicInvoice { get; set; } = default!;
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator(IRepository<Company> repository)
        {
            RuleFor(r => r.CompanyId)
                .NotEmpty();

            WhenAsync(async (request, token) =>
            {
                var companyType = await repository.FirstOrDefaultAsync<CompanyType?>(query => query
                        .Select(c => c.Type)
                        .Where(c => c.Id == request.CompanyId),
                    token
                );

                return companyType == CompanyType.Customer;
            }, () =>
            {
                RuleFor(r => r.ElectronicInvoice)
                    .NotNull()
                    .SetValidator(new BaseSaveElectronicInvoice.Validator()!);
            });

            Include(new BaseValidator());
        }
    }

    internal class BaseValidator : AbstractValidator<BaseRequest>
    {
        public BaseValidator()
        {
            Include(new BaseSaveCompanyBillingTaxes.Validator());
        }
    }

    internal class Handler : IRequestHandler<Request>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly CompanyService _companyService;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            CompanyService companyService
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _companyService = companyService;
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

            _companyService.SetElectronicInvoice(company, request.ElectronicInvoice!);

            var change = company.InitChange();

            change.RetentionSubject = request.RetentionSubject;
            change.RequiredToDeclareIncome = request.RequiredToDeclareIncome;
            change.VatResponsible = request.VatResponsible;

            change.DianGreatContributor = request.DianGreatContributor;
            change.DianGreatContributorRes = request.DianGreatContributor
                ? request.DianGreatContributorRes
                : null;

            change.SalesRetentionAgent = request.SalesRetentionAgent;
            change.SalesRetentionAgentRes = request.SalesRetentionAgent
                ? request.SalesRetentionAgentRes
                : null;

            change.IncomeSelfRetainer = request.IncomeSelfRetainer;
            change.IncomeSelfRetainerRes = request.IncomeSelfRetainer
                ? request.IncomeSelfRetainerRes
                : null;

            change.Regime = request.Regime;
            change.IcaActivity = request.IcaActivity;

            change.IcaAutoRetainer = request.IcaAutoRetainer;
            change.IcaAutoRetainerRes = request.IcaAutoRetainer
                ? request.IcaAutoRetainerRes
                : null;

            company.SetLastOnboardingStep(OnboardingStep.Associates);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
