namespace ApiTemplate.Application.Features.Companies.Dtos;

public class BaseSaveCompanyBillingTaxes
{
    public class Request
    {
        public bool RetentionSubject { get; set; }
        public bool RequiredToDeclareIncome { get; set; }
        public bool VatResponsible { get; set; }
        public bool DianGreatContributor { get; set; }
        public string? DianGreatContributorRes { get; set; }
        public bool SalesRetentionAgent { get; set; }
        public string? SalesRetentionAgentRes { get; set; }
        public bool IncomeSelfRetainer { get; set; }
        public string? IncomeSelfRetainerRes { get; set; }
        public CompanyRegime Regime { get; set; }
        public string? IcaActivity { get; set; }
        public bool IcaAutoRetainer { get; set; }
        public string? IcaAutoRetainerRes { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.DianGreatContributorRes)
                .MaximumLength(30);
            When(r => r.DianGreatContributor, () =>
            {
                RuleFor(r => r.DianGreatContributorRes)
                    .NotEmpty();
            });

            RuleFor(r => r.SalesRetentionAgentRes)
                .MaximumLength(30);
            When(r => r.SalesRetentionAgent, () =>
            {
                RuleFor(r => r.SalesRetentionAgentRes)
                    .NotEmpty();
            });

            RuleFor(r => r.IncomeSelfRetainerRes)
                .MaximumLength(30);
            When(r => r.IncomeSelfRetainer, () =>
            {
                RuleFor(r => r.IncomeSelfRetainerRes)
                    .NotEmpty();
            });

            RuleFor(r => r.Regime)
                .NotEmpty();

            RuleFor(r => r.IcaActivity)
                .NotEmpty()
                .MaximumLength(4);

            RuleFor(r => r.IcaAutoRetainerRes)
                .MaximumLength(30);
            When(r => r.IcaAutoRetainer, () =>
            {
                RuleFor(r => r.IcaAutoRetainerRes)
                    .NotEmpty();
            });
        }
    }
}
