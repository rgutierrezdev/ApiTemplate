namespace ApiTemplate.Domain.Entities;

public class DocumentType : AuditableEntity
{
    public string ShortName { get; set; } = default!;
    public string Name { get; set; } = default!;

    // Navigation properties
    public ICollection<Company> Companies { get; set; } = default!;
    public ICollection<Company> LegalRepresentativeCompanies { get; set; } = default!;
    public ICollection<CompanyAssociate> CompanyAssociates { get; set; } = default!;
    public ICollection<SignedFileSignature> SignedFileSignatures { get; set; } = default!;
    public ICollection<CompanyChange> CompanyChanges { get; set; } = default!;
    public ICollection<CompanyChange> LegalRepresentativeCompanyChanges { get; set; } = default!;
}
