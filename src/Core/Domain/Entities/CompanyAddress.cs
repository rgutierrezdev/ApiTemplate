namespace ApiTemplate.Domain.Entities;

public class CompanyAddress : AuditableEntity
{
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = default!;
    public Guid CityId { get; set; }
    public string Address { get; set; } = default!;
    public string? AdditionalInfo { get; set; }
    public bool Enabled { get; set; }

    // Navigation properties
    public Company Company { get; set; } = default!;
    public City City { get; set; } = default!;
}
