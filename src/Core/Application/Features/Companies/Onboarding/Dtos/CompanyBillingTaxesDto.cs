using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Onboarding.Dtos;

public class CompanyBillingTaxesDto : BaseCompanyBillingTaxesDto
{
    public CompanyType Type { get; set; }
    public ElectronicInvoiceDto ElectronicInvoice { get; set; } = default!;
}
