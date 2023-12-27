namespace ApiTemplate.Domain.Entities;

public class File : AuditableEntity
{
    public string Src { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Mime { get; set; } = default!;
    public long Size { get; set; }
    public bool Public { get; set; }
    public bool MarkForDeletion { get; set; }

    // navigation properties
    public CompanyDocumentFile? CompanyDocumentFile { get; set; }
    public SignedFile? SignedFile { get; set; }
    public ICollection<Company> LogoCompanies { get; set; } = default!;
}
