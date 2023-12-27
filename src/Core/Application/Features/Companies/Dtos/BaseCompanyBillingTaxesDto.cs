namespace ApiTemplate.Application.Features.Companies.Dtos;

public class BaseCompanyBillingTaxesDto
{
    public bool? RetentionSubject { get; set; }
    public bool? RequiredToDeclareIncome { get; set; }
    public bool? VatResponsible { get; set; }
    public bool? DianGreatContributor { get; set; }
    public string? DianGreatContributorRes { get; set; }
    public bool? SalesRetentionAgent { get; set; }
    public string? SalesRetentionAgentRes { get; set; }
    public bool? IncomeSelfRetainer { get; set; }
    public string? IncomeSelfRetainerRes { get; set; }
    public CompanyRegime? Regime { get; set; }
    public string? IcaActivity { get; set; }
    public bool? IcaAutoRetainer { get; set; }
    public string? IcaAutoRetainerRes { get; set; }
}

public record ElectronicInvoiceDto
(
    string? FullName,
    string? PhoneNumber,
    string? Email,
    int? AccountingCloseDay
);
