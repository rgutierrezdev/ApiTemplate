namespace ApiTemplate.Domain.Entities;

public enum CompanySignedFileType
{
    Registration = 1,
}

public class CompanySignedFile : AuditableEntity
{
    public Guid CompanyId { get; set; }
    public CompanySignedFileType Type { get; set; }

    // Navigation properties
    public Company Company { get; set; } = default!;
    public SignedFile SignedFile { get; set; } = default!;
}
