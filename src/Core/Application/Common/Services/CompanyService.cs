using ApiTemplate.Application.Common.Mailing;
using ApiTemplate.Application.Common.Mailing.Templates;
using ApiTemplate.Application.Features.Companies.Dtos;
using ApiTemplate.Application.Features.Companies.Profile.Dtos;

namespace ApiTemplate.Application.Common.Services;

public class CompanyService : IScopedService
{
    private readonly IRepository<Company> _companyRepository;
    private readonly IRepository<CompanyDocument> _companyDocumentRepository;
    private readonly IRepository<CompanyContact> _companyContactsRepository;
    private readonly IMailService _mailService;

    public CompanyService(
        IRepository<Company> companyRepository,
        IRepository<CompanyDocument> companyDocumentRepository,
        IRepository<CompanyContact> companyContactsRepository,
        IMailService mailService
    )
    {
        _companyRepository = companyRepository;
        _companyDocumentRepository = companyDocumentRepository;
        _companyContactsRepository = companyContactsRepository;
        _mailService = mailService;
    }

    public void SetContacts(Company company, BaseSaveCompanyContacts.Request request)
    {
        var mainContact = company.CompanyContacts.FirstOrDefault(cc => cc.Type == CompanyContactType.Main);

        if (mainContact == null)
        {
            mainContact = new CompanyContact()
            {
                Id = Ulid.NewGuid(),
                Name = request.MainContact.Name,
                Email = request.MainContact.Email,
                PhoneNumber = request.MainContact.PhoneNumber,
                Type = CompanyContactType.Main,
                CompanyId = company.Id,
            };

            company.CompanyContacts.Add(mainContact);
        }
        else
        {
            mainContact.Name = request.MainContact.Name;
            mainContact.Email = request.MainContact.Email;
            mainContact.PhoneNumber = request.MainContact.PhoneNumber;
        }

        HandleOptionalContact(company, CompanyContactType.Treasury, request.TreasuryContact);
        HandleOptionalContact(company, CompanyContactType.SalesPurchasing, request.OtherContact);
    }

    private void HandleOptionalContact(
        Company company,
        CompanyContactType contactType,
        BaseSaveCompanyContacts.OptionalContactRequest? contactRequest
    )
    {
        var companyContact = company.CompanyContacts.FirstOrDefault(cc => cc.Type == contactType);

        var isContactRequestEmpty = contactRequest != null &&
                                    string.IsNullOrWhiteSpace(contactRequest.Name) &&
                                    string.IsNullOrWhiteSpace(contactRequest.Email) &&
                                    string.IsNullOrWhiteSpace(contactRequest.PhoneNumber);

        if (companyContact == null)
        {
            if (!isContactRequestEmpty)
            {
                companyContact = new CompanyContact()
                {
                    Id = Ulid.NewGuid(),
                    Name = contactRequest!.Name,
                    Email = contactRequest.Email,
                    PhoneNumber = contactRequest.PhoneNumber,
                    Type = contactType,
                    CompanyId = company.Id,
                };

                company.CompanyContacts.Add(companyContact);
            }
        }
        else
        {
            if (isContactRequestEmpty)
            {
                company.CompanyContacts.Remove(companyContact);
            }
            else
            {
                companyContact.Name = contactRequest!.Name;
                companyContact.Email = contactRequest.Email;
                companyContact.PhoneNumber = contactRequest.PhoneNumber;
            }
        }
    }

    public void SetElectronicInvoice(Company company, BaseSaveElectronicInvoice.Request request)
    {
        if (company.Type == CompanyType.Customer)
        {
            company.EInvoiceFullName = request.FullName;
            company.EInvoicePhoneNumber = request.PhoneNumber;
            company.EInvoiceEmail = request.Email;
            company.EInvoiceAccountingCloseDay = request.AccountingCloseDay;
        }
        else
        {
            company.EInvoiceFullName = null;
            company.EInvoicePhoneNumber = null;
            company.EInvoiceEmail = null;
            company.EInvoiceAccountingCloseDay = null;
        }
    }

    private class CompanyContactData : CompanyContactDto
    {
        public CompanyContactType Type { get; set; }
    }

    public async Task<BaseCompanyContactsDto> GetContactsAsync(
        Guid companyId,
        CancellationToken cancellationToken = default
    )
    {
        var contacts = await _companyContactsRepository.ListAsync<CompanyContactData>(query =>
                query.Where(cc => cc.CompanyId == companyId),
            cancellationToken
        );

        var contactsDto = new BaseCompanyContactsDto()
        {
            MainContact = contacts.Find(cc => cc.Type == CompanyContactType.Main)
                          ?? new CompanyContactDto(),
            TreasuryContact = contacts.Find(cc => cc.Type == CompanyContactType.Treasury)
                              ?? new CompanyContactDto(),
            OtherContact = contacts.Find(cc => cc.Type == CompanyContactType.SalesPurchasing)
                           ?? new CompanyContactDto(),
        };

        return contactsDto;
    }

    private class CompanyData
    {
        public bool? CreditEnabled { get; set; }
        public string? CreditReviewMessage { get; set; }
        public ICollection<CompanyDocumentFile> CompanyDocumentsFiles { get; init; } = default!;
        public CompanyChange? CompanyChange { get; init; }
    }

    public async Task<CompanyProfilePaymentTypeDto> GetDocumentsAsync(
        Guid companyId,
        bool creditDocuments,
        CancellationToken cancellationToken = default
    )
    {
        var company = await _companyRepository.FirstOrDefaultAsync<CompanyData>(
            query => query
                .Select(c => new CompanyData()
                {
                    CreditEnabled = c.CreditEnabled,
                    CreditReviewMessage = c.CreditReviewMessage,
                    CompanyDocumentsFiles = c.CompanyDocumentsFiles,
                    CompanyChange = c.CompanyChange,
                })
                .Include(c => c.CompanyDocumentsFiles)
                .ThenInclude(cdf => cdf.File)
                .Where(c => c.Id == companyId),
            cancellationToken
        ) ?? throw new NotFoundException(
            ErrorCodes.CompanyNotFound,
            $"Company with id '{companyId}' was not found"
        );

        var companyDocuments = await _companyDocumentRepository.ListAsync<CompanyProfileDocumentDto>(query =>
            {
                if (creditDocuments)
                    query.Where(cd => cd.CreditEnabled == true);
                else
                    query.Where(cd => cd.CreditEnabled == null || cd.CreditEnabled == false);
            },
            cancellationToken
        );

        foreach (var companyDocument in companyDocuments)
        {
            companyDocument.Files = GetCompanyProfileDocumentFiles(
                company.CompanyDocumentsFiles,
                companyDocument.Id
            );
        }

        return new CompanyProfilePaymentTypeDto()
        {
            Documents = companyDocuments,
            CompanyInfoReviewStatus = company.CompanyChange?.CompanyInfoReviewStatus,
            BillingTaxesReviewStatus = company.CompanyChange?.CompanyInfoReviewStatus,
            AssociatesReviewStatus = company.CompanyChange?.CompanyInfoReviewStatus,
            DocumentsReviewStatus = company.CompanyChange?.DocumentsReviewStatus,
            CreditReviewStatus = company.CompanyChange?.CreditReviewStatus,
            Current = new CompanyProfilePaymentTypeDto.CurrentChange()
            {
                CreditEnabled = company.CreditEnabled,
                CreditReviewMessage = company.CreditReviewMessage,
            },
            Change = new CompanyProfilePaymentTypeDto.CurrentChange()
            {
                CreditEnabled = company.CompanyChange?.CreditEnabled,
                CreditReviewMessage = company.CompanyChange?.CreditReviewMessage,
            }
        };
    }

    private static List<CompanyProfileDocumentFileDto> GetCompanyProfileDocumentFiles(
        IEnumerable<CompanyDocumentFile> companyDocumentFiles,
        Guid companyDocumentId
    )
    {
        var documentFiles = companyDocumentFiles.ToList();

        return documentFiles
            .Where(cdf => cdf.CompanyDocumentId == companyDocumentId && cdf.ChangeCompanyDocumentFileId == null)
            .Select(cdf => new CompanyProfileDocumentFileDto
            {
                Current = new CompanyProfileDocumentFileDto.CurrentChange()
                {
                    Id = cdf.Id,
                    Name = cdf.File.Name,
                    Status = cdf.Status,
                    ReviewMessage = cdf.ReviewMessage,
                },
                Change = cdf.ChangedByCompanyDocumentFile != null
                    ? new CompanyProfileDocumentFileDto.CurrentChange()
                    {
                        Id = cdf.ChangedByCompanyDocumentFile.Id,
                        Name = cdf.ChangedByCompanyDocumentFile.File.Name,
                        Status = cdf.ChangedByCompanyDocumentFile.Status,
                        ReviewMessage = cdf.ChangedByCompanyDocumentFile.ReviewMessage,
                    }
                    : null,
            })
            .ToList();
    }

    public async Task SendSignRegistrationEmailAsync(
        Guid companyId,
        string signerEmail,
        string companyName,
        string signerFirstName,
        string signerLastName,
        string signOnboardingToken,
        CancellationToken cancellationToken = default
    )
    {
        var hashedSignToken = BCrypt.Net.BCrypt.HashPassword(signOnboardingToken);

        await _mailService.SendTemplatedEmail(new[] { signerEmail },
            new SignCompanyRegistrationTemplate(
                new SignCompanyRegistrationTemplate.Variables()
                {
                    CompanyName = companyName,
                    RepFirstName = signerFirstName,
                    RepLastName = signerLastName,
                    SignDocumentLink = new AppLink(
                        $"/sign-document/company-registration?token={Uri.EscapeDataString(hashedSignToken)}&companyId={companyId}"
                    )
                }),
            cancellationToken
        );
    }
}
