namespace ApiTemplate.Domain.Entities;

public class CostCenterUser : AuditableEntity
{
    public Guid CostCenterId { get; set; }
    public Guid CompanyUserId { get; set; }

    // Navigation properties
    public CostCenter CostCenter { get; set; } = default!;
    public CompanyUser CompanyUser { get; set; } = default!;
}
