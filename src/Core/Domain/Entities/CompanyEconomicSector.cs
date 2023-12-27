namespace ApiTemplate.Domain.Entities;

public class CompanyEconomicSector : AuditableWeakEntity
{
    public Guid CompanyId { get; set; }
    public Guid EconomicSectorId { get; set; }

    // Navigation properties
    public Company Company { get; set; } = default!;
    public EconomicSector EconomicSector { get; set; } = default!;
}
