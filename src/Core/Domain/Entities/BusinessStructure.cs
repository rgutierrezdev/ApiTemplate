namespace ApiTemplate.Domain.Entities;

public class BusinessStructure : AuditableEntity
{
    public Guid CountryId { get; set; }
    public string Name { get; set; } = default!;

    // Navigation properties
    public Country Country { get; set; } = default!;
    public ICollection<Company> Companies { get; set; } = default!;
    public ICollection<CompanyChange> CompanyChanges { get; set; } = default!;
}
