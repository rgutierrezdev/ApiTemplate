namespace ApiTemplate.Domain.Entities;

public class EconomicSector : AuditableEntity
{
    public string Name { get; set; } = default!;

    // Navigation properties
    public ICollection<CompanyEconomicSector> CompanyEconomicSectors { get; set; } = default!;
}
