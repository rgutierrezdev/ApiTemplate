namespace ApiTemplate.Domain.Entities;

public class City : AuditableEntity
{
    public Guid StateId { get; set; }
    public string Name { get; set; } = default!;
    public string? DaneCode { get; set; }

    // Navigation properties
    public State State { get; set; } = default!;
    public ICollection<Company> Companies { get; set; } = default!;
    public ICollection<CompanyChange> CompanyChanges { get; set; } = default!;
    public ICollection<CompanyAddress> CompanyAddresses { get; set; } = default!;
}
