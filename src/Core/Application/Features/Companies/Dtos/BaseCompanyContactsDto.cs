namespace ApiTemplate.Application.Features.Companies.Dtos;

public class BaseCompanyContactsDto
{
    public CompanyContactDto MainContact { get; set; } = default!;
    public CompanyContactDto TreasuryContact { get; set; } = default!;
    public CompanyContactDto OtherContact { get; set; } = default!;
}

public class CompanyContactDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
