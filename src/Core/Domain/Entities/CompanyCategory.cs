namespace ApiTemplate.Domain.Entities;

public class CompanyCategory : AuditableWeakEntity
{
    public Guid CompanyId { get; set; }
    public Guid CategoryId { get; set; }

    // Navigation properties
    public Company Company { get; set; } = default!;
    public Category Category { get; set; } = default!;
}
