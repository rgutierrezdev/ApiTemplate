namespace ApiTemplate.Domain.Entities;

public class CompanyAssociate : AuditableEntity
{
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = default!;
    public Guid DocumentTypeId { get; set; }
    public string Document { get; set; } = default!;
    public int ParticipationPercent { get; set; }
    public bool Pep { get; set; }
    public bool IsChange { get; set; }

    // Navigation properties
    public Company Company { get; set; } = default!;
    public DocumentType DocumentType { get; set; } = default!;
}
