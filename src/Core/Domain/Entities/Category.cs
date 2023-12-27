namespace ApiTemplate.Domain.Entities;

public class Category : AuditableEntity
{
    public string Name { get; set; } = default!;

    // Navigation properties
    public ICollection<CompanyCategory> CompanyCategories { get; set; } = default!;
}
