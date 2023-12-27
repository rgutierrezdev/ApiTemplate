using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Onboarding.Dtos;

public class CompanyBasicInfoDto : BaseCompanyBasicInfoDto
{
    public CompanyType? Type { get; set; }
    public CompanyCouponDto? Coupon { get; set; }
}
