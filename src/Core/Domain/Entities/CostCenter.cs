namespace ApiTemplate.Domain.Entities;

public class CostCenter : AuditableEntity
{
    public const string DefaultName = "General";

    // props
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = default!;
    public bool Default { get; set; }
    public bool Enabled { get; set; }

    // Navigation properties
    public Company Company { get; set; } = default!;
    public ICollection<CostCenterUser> CostCenterUsers { get; set; } = default!;
}
