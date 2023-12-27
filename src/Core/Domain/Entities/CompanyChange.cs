namespace ApiTemplate.Domain.Entities;

public class CompanyChange : AuditableEntity
{
    public CompanyLegalType? LegalType { get; set; }
    public string? LegalName { get; set; }
    public string? CiiuCode { get; set; }
    public PersonType? PersonType { get; set; }
    public Guid? BusinessStructureId { get; set; }
    public Guid? DocumentTypeId { get; set; }
    public string? Document { get; set; }
    public string? VerificationDigit { get; set; }

    public Guid? CityId { get; set; }
    public string? Address { get; set; }

    public string? LegalRepresentativeFirstName { get; set; }
    public string? LegalRepresentativeLastName { get; set; }
    public string? LegalRepresentativeEmail { get; set; }
    public Guid? LegalRepresentativeDocumentTypeId { get; set; }
    public string? LegalRepresentativeDocument { get; set; }

    public ReviewStatus? DocumentsReviewStatus { get; set; }

    public ReviewStatus? CompanyInfoReviewStatus { get; set; }
    public string? CompanyInfoReviewMessage { get; set; }

    public bool? RetentionSubject { get; set; }
    public bool? RequiredToDeclareIncome { get; set; }
    public bool? VatResponsible { get; set; }
    public bool? DianGreatContributor { get; set; }
    public string? DianGreatContributorRes { get; set; }
    public bool? SalesRetentionAgent { get; set; }
    public string? SalesRetentionAgentRes { get; set; }
    public bool? IncomeSelfRetainer { get; set; }
    public string? IncomeSelfRetainerRes { get; set; }
    public CompanyRegime? Regime { get; set; }
    public string? IcaActivity { get; set; }
    public bool? IcaAutoRetainer { get; set; }
    public string? IcaAutoRetainerRes { get; set; }
    public ReviewStatus? BillingTaxesReviewStatus { get; set; }
    public string? BillingTaxesReviewMessage { get; set; }

    public bool? CreditEnabled { get; set; }
    public bool? AuthorizesFinancialInformation { get; set; }
    public ReviewStatus? CreditReviewStatus { get; set; }
    public string? CreditReviewMessage { get; set; }

    public bool? HasPepRelative { get; set; }
    public bool? UnderOath { get; set; }
    public ReviewStatus? AssociatesReviewStatus { get; set; }
    public string? AssociatesReviewMessage { get; set; }

    // Navigation properties
    public Company Company { get; set; } = default!;
    public BusinessStructure? BusinessStructure { get; set; }
    public DocumentType? DocumentType { get; set; }
    public City? City { get; set; }
    public DocumentType? LegalRepresentativeDocumentType { get; set; }
}
