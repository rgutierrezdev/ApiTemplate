using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile.Dtos;

public class CompanyProfileBasicInfoDto : BaseProfileResponse
{
    public CurrentChange Current { get; set; } = default!;
    public CurrentChange Change { get; set; } = default!;

    public class CurrentChange : BaseCompanyBasicInfoDto
    {
        public ReviewStatus? CompanyInfoReviewStatus { get; set; }
        public string? CompanyInfoReviewMessage { get; set; }
    }
}
