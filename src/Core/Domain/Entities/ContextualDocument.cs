namespace ApiTemplate.Domain.Entities;

public enum ContextualDocumentType
{
    CustomerTermsAndConditions = 1,
    VendorTermsAndConditions = 2,
    PrivacyPolicy = 3,
}

public class ContextualDocument : AuditableEntity
{
    public ContextualDocumentType Type { get; set; }
    public int Version { get; set; }
    public string Content { get; set; } = default!;
}
