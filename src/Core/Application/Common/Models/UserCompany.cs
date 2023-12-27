using ApiTemplate.Application.Features.Auth.Dtos;

namespace ApiTemplate.Application.Common.Models;

public class UserCompany : AuthCompanyDto
{
    public Guid? LogoFileId { get; set; }
    public bool? LogoPublic { get; set; }
    public Guid CompanyUserId { get; set; }
}
