namespace ApiTemplate.Application.Features.Companies.Profile.Dtos;

public class CompanyProfilePaymentTypeDto : CompanyProfileDocumentsDto
{
    public CurrentChange Current { get; set; } = default!;
    public CurrentChange Change { get; set; } = default!;

    public class CurrentChange
    {
        public bool? CreditEnabled { get; set; }
        public string? CreditReviewMessage { get; set; }
    }
}
