namespace ApiTemplate.Domain.Entities;

public class State : AuditableEntity
{
    public Guid CountryId { get; set; }
    public string Name { get; set; } = default!;
    public string? DaneCode { get; set; }

    // Navigation properties
    public Country Country { get; set; } = default!;
    public ICollection<City> Cities { get; set; } = default!;
}
