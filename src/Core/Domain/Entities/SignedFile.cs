namespace ApiTemplate.Domain.Entities;

public class SignedFile : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string Hash { get; set; } = default!;

    // navigation properties
    public File File { get; set; } = default!;
    public CompanySignedFile? CompanySignedFile { get; set; }
    public ICollection<SignedFileSignature> SignedFileSignatures { get; set; } = default!;
}
