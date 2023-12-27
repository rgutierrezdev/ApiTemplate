namespace ApiTemplate.Domain.Entities;

public class SignedFileSignature : AuditableEntity
{
    public Guid SignedFileId { get; set; }
    public Guid DocumentTypeId { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? IpAddress { get; set; }
    public string? Client { get; set; }
    public string? Token { get; set; }
    public DateTime SignedDate { get; set; }

    // navigation properties
    public SignedFile SignedFile { get; set; } = default!;
    public DocumentType DocumentType { get; set; } = default!;
}
