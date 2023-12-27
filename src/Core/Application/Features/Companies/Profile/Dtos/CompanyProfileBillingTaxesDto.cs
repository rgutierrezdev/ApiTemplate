using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile.Dtos;

public class CompanyProfileBillingTaxesDto : BaseProfileResponse
{
    public CurrentChangeElectronic Current { get; set; } = default!;
    public CurrentChange Change { get; set; } = default!;

    public class CurrentChange : BaseCompanyBillingTaxesDto
    {
        public ReviewStatus? BillingTaxesReviewStatus { get; set; }
        public string? BillingTaxesReviewMessage { get; set; }
    }

    public class CurrentChangeElectronic : CurrentChange
    {
        public ElectronicInvoiceDto ElectronicInvoice { get; set; } = default!;
    }
}
