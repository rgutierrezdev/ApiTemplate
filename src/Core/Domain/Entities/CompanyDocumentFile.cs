namespace ApiTemplate.Domain.Entities;

public enum ReviewStatus
{
    ReadyToReview = 1,
    Reviewing = 2,
    Approved = 3,
    Rejected = 4,
}

public class CompanyDocumentFile : AuditableEntity
{
    public Guid CompanyId { get; set; }
    public Guid CompanyDocumentId { get; set; }
    public ReviewStatus Status { get; set; }
    public string? ReviewMessage { get; set; }
    public Guid? ChangeCompanyDocumentFileId { get; set; }

    // Navigation properties
    public Company Company { get; set; } = default!;
    public CompanyDocument CompanyDocument { get; set; } = default!;
    public CompanyDocumentFile? ChangeCompanyDocumentFile { get; set; }
    public CompanyDocumentFile? ChangedByCompanyDocumentFile { get; set; }
    public File File { get; set; } = default!;
}
