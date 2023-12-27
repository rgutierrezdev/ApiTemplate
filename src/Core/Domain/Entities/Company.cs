namespace ApiTemplate.Domain.Entities;

public enum CompanyStatus
{
    OnBoarding = 1,
    Reviewing = 2,
    Approved = 3,
    Rejected = 4,
}

public enum CompanyType
{
    Customer = 1,
    Vendor = 2,
}

public enum CompanyLegalType
{
    Private = 1,
    Public = 2,
    Mixed = 3,
}

public enum PersonType
{
    Legal = 1,
    Natural = 2,
}

public enum OnboardingStep
{
    BasicInformation = 1,
    LegalNotices = 2,
    PaymentType = 3,
    Documents = 4,
    ContactInfo = 5,
    BillingAndTaxes = 6,
    Associates = 7,
    Summary = 8,
}

public enum CompanyRegime
{
    VatResponsible = 1,
    NoVatResponsible = 2,
    Special = 3,
    Simple = 4
}

public enum EmployeesNumber
{
    From0To10 = 1,
    From10To50 = 2,
    From50To100 = 3,
    From100To250 = 4,
    MoreThan250 = 5,
}

public enum YearlyIncome
{
    From0MTo500M = 1,
    From500MTo1000M = 2,
    From1000MTo10000M = 3,
    From10000MTo70000M = 4,
    MoreThan70000M = 5,
}

public class Company : AuditableEntity
{
    public string Name { get; set; } = default!;
    public CompanyStatus Status { get; set; }
    public string? ContactPhoneNumber { get; set; }
    public bool UsesPurchaseOrder { get; set; }

    public OnboardingStep? LastOnboardingStep { get; set; }

    public CompanyLegalType? LegalType { get; set; }
    public CompanyType? Type { get; set; }
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
    public string? SignOnboardingToken { get; set; }

    public ReviewStatus? DocumentsReviewStatus { get; set; }

    public ReviewStatus? CompanyInfoReviewStatus { get; set; }
    public string? CompanyInfoReviewMessage { get; set; }

    public bool? AgreesTermsAndConditions { get; set; }

    public bool CreditEnabled { get; set; }
    public bool? AuthorizesFinancialInformation { get; set; }
    public decimal? CreditLimit { get; set; }
    public ReviewStatus? CreditReviewStatus { get; set; }
    public string? CreditReviewMessage { get; set; }

    public string? EInvoiceFullName { get; set; }
    public string? EInvoicePhoneNumber { get; set; }
    public string? EInvoiceEmail { get; set; }
    public int? EInvoiceAccountingCloseDay { get; set; }

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

    public bool? HasPepRelative { get; set; }
    public bool? UnderOath { get; set; }
    public ReviewStatus? AssociatesReviewStatus { get; set; }
    public string? AssociatesReviewMessage { get; set; }

    public Guid? LogoFileId { get; set; }
    public EmployeesNumber? EmployeesNumber { get; set; }
    public YearlyIncome? YearlyIncome { get; set; }
    public DateOnly? ConstitutionDate { get; set; }
    public string? AboutUs { get; set; }
    public string? ContactEmail { get; set; }
    public string? WebsiteUrl { get; set; }

    // Navigation properties
    public BusinessStructure? BusinessStructure { get; set; }
    public DocumentType? DocumentType { get; set; }
    public City? City { get; set; }

    public DocumentType? LegalRepresentativeDocumentType { get; set; }

    public File? LogoFile { get; set; }
    public CompanyChange? CompanyChange { get; set; }
    public ICollection<CostCenter> CostCenters { get; set; } = default!;
    public ICollection<CompanyUser> CompanyUsers { get; set; } = default!;
    public ICollection<CompanyDocumentFile> CompanyDocumentsFiles { get; set; } = default!;
    public ICollection<CompanyContact> CompanyContacts { get; set; } = default!;
    public ICollection<CompanyAssociate> CompanyAssociates { get; set; } = default!;
    public ICollection<CompanyEconomicSector> CompanyEconomicSectors { get; set; } = default!;
    public ICollection<CompanyCategory> CompanyCategories { get; set; } = default!;
    public ICollection<CompanyAddress> CompanyAddresses { get; set; } = default!;
    public ICollection<CompanySignedFile> CompanySignedFiles { get; set; } = default!;

    public void SetLastOnboardingStep(OnboardingStep onboardingStep)
    {
        if (LastOnboardingStep == null || (int)onboardingStep > (int)LastOnboardingStep)
        {
            LastOnboardingStep = onboardingStep;
        }
    }

    public CompanyChange InitChange()
    {
        CompanyChange ??= new CompanyChange()
        {
            Id = Id
        };

        return CompanyChange;
    }
}
