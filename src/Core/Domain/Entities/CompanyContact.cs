namespace ApiTemplate.Domain.Entities;

public enum CompanyContactType
{
    Main = 1,
    Treasury = 2,
    SalesPurchasing = 3,
}

public class CompanyContact : AuditableEntity
{
    public Guid CompanyId { get; set; }
    public CompanyContactType Type { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    // Navigation properties
    public Company Company { get; set; } = default!;
}
