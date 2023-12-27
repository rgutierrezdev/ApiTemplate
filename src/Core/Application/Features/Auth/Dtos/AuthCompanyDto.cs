namespace ApiTemplate.Application.Features.Auth.Dtos;

public class AuthCompanyDto
{
    public Guid Id { get; set; }
    public CompanyType? Type { get; set; }
    public string Name { get; set; } = default!;
    public string? LegalName { get; set; }
    public string? CountryName { get; set; }
    public string? CountryIsoCode { get; set; }
    public string? StateName { get; set; }
    public string? CityName { get; set; }
    public string? Address { get; set; }
    public bool UsesPurchaseOrder { get; set; }
    public CompanyStatus Status { get; set; }
    public OnboardingStep? LastOnboardingStep { get; set; }
    public string? LogoUrl { get; set; }
    public ICollection<string> Permissions { get; set; } = default!;
    public ICollection<AuthCostCenterDto> CostCenters { get; set; } = default!;
}
