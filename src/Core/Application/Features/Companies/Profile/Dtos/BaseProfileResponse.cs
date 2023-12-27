namespace ApiTemplate.Application.Features.Companies.Profile.Dtos;

public class BaseProfileResponse
{
    public ReviewStatus? CompanyInfoReviewStatus { get; set; }
    public ReviewStatus? BillingTaxesReviewStatus { get; set; }
    public ReviewStatus? AssociatesReviewStatus { get; set; }
    public ReviewStatus? DocumentsReviewStatus { get; set; }
    public ReviewStatus? CreditReviewStatus { get; set; }
}
