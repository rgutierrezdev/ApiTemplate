using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile.Dtos;

public class CompanyProfileAssociatesDto : BaseProfileResponse
{
    public CurrentChange Current { get; set; } = default!;
    public CurrentChange Change { get; set; } = default!;

    public class CurrentChange : BaseCompanyAssociatesDto
    {
        public ReviewStatus? AssociatesReviewStatus { get; set; }
        public string? AssociatesReviewMessage { get; set; }
    }
}
