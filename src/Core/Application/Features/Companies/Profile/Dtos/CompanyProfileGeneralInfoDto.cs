namespace ApiTemplate.Application.Features.Companies.Profile.Dtos;

public class CompanyProfileGeneralInfoDto : BaseProfileResponse
{
    public CurrenChange Current { get; set; } = default!;

    public class CurrenChange
    {
        public EmployeesNumber? EmployeesNumber { get; set; }
        public YearlyIncome? YearlyIncome { get; set; }
        public DateOnly? ConstitutionDate { get; set; }
        public string? AboutUs { get; set; }
        public string? ContactEmail { get; set; }
        public string? WebsiteUrl { get; set; }
        public ICollection<Guid> EconomicSectorIds { get; set; } = default!;
        public ICollection<Guid> CategoryIds { get; set; } = default!;
    }
}
