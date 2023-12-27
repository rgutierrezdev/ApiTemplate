namespace ApiTemplate.Domain.Entities;

public class CompanyDocument : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool? CreditEnabled { get; set; }
    public short MinQuantity { get; set; }
    public short MaxQuantity { get; set; }

    // Navigation properties
    public ICollection<CompanyDocumentFile> CompanyDocumentsFiles { get; set; } = default!;
}
