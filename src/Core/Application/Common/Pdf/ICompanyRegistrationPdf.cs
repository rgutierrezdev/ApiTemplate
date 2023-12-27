namespace ApiTemplate.Application.Common.Pdf;

public interface ICompanyRegistrationPdf : IPdfGeneratorService<CompanyRegistrationPdfModel>
{
}

public class CompanyRegistrationPdfModel
{
    public CompanyType? Type { get; set; }
    public string? LegalName { get; set; }
    public PersonType? PersonType { get; set; }
    public string? DocumentTypeName { get; set; }
    public string? DocumentTypeShortName { get; set; }
    public string? Document { get; set; }
    public string? VerificationDigit { get; set; }

    public string? LegalRepresentativeFirstName { get; set; }
    public string? LegalRepresentativeLastName { get; set; }
    public string? LegalRepresentativeEmail { get; set; }
    public Guid? LegalRepresentativeDocumentTypeId { get; set; }
    public string? LegalRepresentativeDocumentTypeShortName { get; set; }
    public string? LegalRepresentativeDocumentTypeName { get; set; }
    public string? LegalRepresentativeDocument { get; set; }
    public string? SignOnboardingToken { get; set; }

    public ICollection<OnboardingSummaryAssociate> Associates { get; set; } = default!;
    public ICollection<OnboardingSummaryContact> Contacts { get; set; } = default!;

    public string? EInvoiceFullName { get; set; }
    public string? EInvoicePhoneNumber { get; set; }
    public string? EInvoiceEmail { get; set; }
    public int? EInvoiceAccountingCloseDay { get; set; }

    public bool? HasPepRelative { get; set; }
    public bool? UnderOath { get; set; }
    public bool? AuthorizesFinancialInformation { get; set; }
    public bool? AgreesTermsAndConditions { get; set; }
    public bool? RetentionSubject { get; set; }
    public bool? RequiredToDeclareIncome { get; set; }
    public bool? VatResponsible { get; set; }
    public bool? DianGreatContributor { get; set; }
    public bool? SalesRetentionAgent { get; set; }
    public bool? IncomeSelfRetainer { get; set; }
    public bool? IcaAutoRetainer { get; set; }
    public CompanyRegime? Regime { get; set; }
    public string? IcaActivity { get; set; }

    public bool IsRegistrationSigned { get; set; }

    public SignatureDto? Signature { get; set; }
}

public class SignatureDto
{
    public string DocumentTypeShortName { get; set; } = default!;
    public string Document { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? IpAddress { get; set; }
    public string? Client { get; set; }
    public string Token { get; set; } = default!;
    public DateTime SignedDate { get; set; }
    public Guid SignedFileId { get; set; }
    public string FileName { get; set; } = default!;
}

public record OnboardingSummaryContact(
    CompanyContactType Type,
    string? Name,
    string? Email,
    string? PhoneNumber
);

public record OnboardingSummaryAssociate
(
    string Name,
    string DocumentTypeShortName,
    string Document,
    int ParticipationPercent,
    bool Pep
);
