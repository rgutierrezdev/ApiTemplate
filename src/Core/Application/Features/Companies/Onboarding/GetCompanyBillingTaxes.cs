using ApiTemplate.Application.Features.Companies.Dtos;
using ApiTemplate.Application.Features.Companies.Onboarding.Dtos;

namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class GetCompanyBillingTaxes
{
    public class Request : IRequest<CompanyBillingTaxesDto>
    {
        public Guid Id { get; set; }

        public Request(Guid id)
        {
            Id = id;
        }
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, CompanyBillingTaxesDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly ICurrentUser _currentUser;

        public Handler(IValidator<Request> validator, IRepository<Company> companyRepository, ICurrentUser currentUser)
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _currentUser = currentUser;
        }

        public async Task<CompanyBillingTaxesDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync<CompanyBillingTaxesDto>(
                query => query
                    .Select(c => new CompanyBillingTaxesDto()
                    {
                        Type = (CompanyType)c.Type!,
                        ElectronicInvoice = new ElectronicInvoiceDto(
                            c.EInvoiceFullName,
                            c.EInvoiceEmail,
                            c.EInvoicePhoneNumber,
                            c.EInvoiceAccountingCloseDay
                        ),
                        RetentionSubject = c.CompanyChange!.RetentionSubject,
                        RequiredToDeclareIncome = c.CompanyChange.RequiredToDeclareIncome,
                        VatResponsible = c.CompanyChange.VatResponsible,
                        DianGreatContributor = c.CompanyChange.DianGreatContributor,
                        DianGreatContributorRes = c.CompanyChange.DianGreatContributorRes,
                        SalesRetentionAgent = c.CompanyChange.SalesRetentionAgent,
                        SalesRetentionAgentRes = c.CompanyChange.SalesRetentionAgentRes,
                        IncomeSelfRetainer = c.CompanyChange.IncomeSelfRetainer,
                        IncomeSelfRetainerRes = c.CompanyChange.IncomeSelfRetainerRes,
                        Regime = c.CompanyChange.Regime,
                        IcaActivity = c.CompanyChange.IcaActivity,
                        IcaAutoRetainer = c.CompanyChange.IcaAutoRetainer,
                        IcaAutoRetainerRes = c.CompanyChange.IcaAutoRetainerRes,
                    })
                    .Where(c => c.Id == request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.Id}' was not found"
            );

            return company;
        }
    }
}
