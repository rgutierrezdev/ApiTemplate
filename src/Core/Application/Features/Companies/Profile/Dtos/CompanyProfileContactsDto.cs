using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile.Dtos;

public class CompanyProfileContactsDto : BaseProfileResponse
{
    public BaseCompanyContactsDto Current { get; set; } = default!;
}
