using ApiTemplate.Application.Features.Companies.Dtos;
using ApiTemplate.Application.Features.Companies.Profile.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class GetCompanyProfileBillingTaxes
{
    public class Request : IRequest<CompanyProfileBillingTaxesDto>
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

    internal class Handler : IRequestHandler<Request, CompanyProfileBillingTaxesDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly ICurrentUser _currentUser;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            ICurrentUser currentUser
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _currentUser = currentUser;
        }

        public async Task<CompanyProfileBillingTaxesDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync<CompanyProfileBillingTaxesDto>(
                query => query
                    .Select(c => new CompanyProfileBillingTaxesDto()
                    {
                        Current = new CompanyProfileBillingTaxesDto.CurrentChangeElectronic()
                        {
                            ElectronicInvoice = new ElectronicInvoiceDto(
                                c.EInvoiceFullName,
                                c.EInvoiceEmail,
                                c.EInvoicePhoneNumber,
                                c.EInvoiceAccountingCloseDay
                            ),
                            RetentionSubject = c.RetentionSubject,
                            RequiredToDeclareIncome = c.RequiredToDeclareIncome,
                            VatResponsible = c.VatResponsible,
                            DianGreatContributor = c.DianGreatContributor,
                            DianGreatContributorRes = c.DianGreatContributorRes,
                            SalesRetentionAgent = c.SalesRetentionAgent,
                            SalesRetentionAgentRes = c.SalesRetentionAgentRes,
                            IncomeSelfRetainer = c.IncomeSelfRetainer,
                            IncomeSelfRetainerRes = c.IncomeSelfRetainerRes,
                            Regime = c.Regime,
                            IcaActivity = c.IcaActivity,
                            IcaAutoRetainer = c.IcaAutoRetainer,
                            IcaAutoRetainerRes = c.IcaAutoRetainerRes,
                            BillingTaxesReviewStatus = c.BillingTaxesReviewStatus,
                            BillingTaxesReviewMessage = c.BillingTaxesReviewMessage,
                        },
                        Change = new CompanyProfileBillingTaxesDto.CurrentChange()
                        {
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
                            BillingTaxesReviewStatus = c.CompanyChange.BillingTaxesReviewStatus,
                            BillingTaxesReviewMessage = c.CompanyChange.BillingTaxesReviewMessage,
                        },
                        CompanyInfoReviewStatus = c.CompanyChange.CompanyInfoReviewStatus,
                        BillingTaxesReviewStatus = c.CompanyChange.BillingTaxesReviewStatus,
                        AssociatesReviewStatus = c.CompanyChange.AssociatesReviewStatus,
                        DocumentsReviewStatus = c.CompanyChange.DocumentsReviewStatus,
                        CreditReviewStatus = c.CompanyChange.CreditReviewStatus,
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
