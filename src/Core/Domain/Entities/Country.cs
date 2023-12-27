namespace ApiTemplate.Domain.Entities;

public class Country : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string IsoCode { get; set; } = default!;

    // Navigation properties
    public ICollection<State> States { get; set; } = default!;
    public ICollection<BusinessStructure> BusinessStructures { get; set; } = default!;
}
