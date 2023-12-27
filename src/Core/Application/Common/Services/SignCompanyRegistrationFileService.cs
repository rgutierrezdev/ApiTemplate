using ApiTemplate.Application.Common.Services.Files;

namespace ApiTemplate.Application.Common.Services;

public class SignCompanyRegistrationFileService : IScopedService
{
    private readonly IRepository<Company> _companyRepository;
    private readonly IRepository<SignedFile> _signedFileRepository;
    private readonly ICompanyRegistrationPdf _companyRegistrationPdf;
    private readonly ICurrentUser _currentUser;
    private readonly FileService _fileService;
    private readonly IUtilsService _utilsService;

    public SignCompanyRegistrationFileService(
        IRepository<Company> companyRepository,
        IRepository<SignedFile> signedFileRepository,
        ICompanyRegistrationPdf companyRegistrationPdf,
        ICurrentUser currentUser,
        FileService fileService,
        IUtilsService utilsService
    )
    {
        _companyRepository = companyRepository;
        _signedFileRepository = signedFileRepository;
        _companyRegistrationPdf = companyRegistrationPdf;
        _currentUser = currentUser;
        _fileService = fileService;
        _utilsService = utilsService;
    }

    public async Task<SignedFile> GenerateAndSignAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var company = await _companyRepository.FirstOrDefaultAsync(
            new CompanyRegistrationByIdSpec(companyId),
            cancellationToken
        ) ?? throw new NotFoundException(
            ErrorCodes.CompanyNotFound,
            $"Company with id '{companyId}' was not found"
        );

        var signedFileId = Ulid.NewGuid();
        var signedDate = DateTime.UtcNow;
        var fileName = "Registro de Empresa.pdf";

        company.Signature = new SignatureDto()
        {
            IpAddress = _currentUser.IpAddress,
            Client = _currentUser.Client,
            Token = company.SignOnboardingToken!,
            SignedDate = signedDate,
            SignedFileId = signedFileId,
            FileName = fileName,
        };

        if (company.PersonType == PersonType.Legal)
        {
            company.Signature.DocumentTypeShortName = company.LegalRepresentativeDocumentTypeShortName!;
            company.Signature.Document = company.LegalRepresentativeDocument!;
            company.Signature.FullName = company.LegalRepresentativeFirstName
                                         + " " + company.LegalRepresentativeLastName;
            company.Signature.Email = company.LegalRepresentativeEmail!;
        }
        else
        {
            company.Signature.DocumentTypeShortName = company.DocumentTypeName!;
            company.Signature.Document = company.Document!;
            company.Signature.FullName = company.LegalName!;
            company.Signature.Email = _currentUser.Email!;
        }

        var pdf = _companyRegistrationPdf.Generate(company);

        var (file, _) = await _fileService.UploadNewAsync(
            signedFileId,
            new BaseFileRequest()
            {
                Name = fileName,
                Base64 = Convert.ToBase64String(pdf),
                Mime = MimeTypes.Pdf,
            },
            false,
            new CompanyRegistrationsFolder(companyId),
            cancellationToken
        );

        var signedFile = new SignedFile()
        {
            Id = file.Id,
            Name = file.Name,
            Hash = _utilsService.GetFileHash(pdf),
            SignedFileSignatures = new List<SignedFileSignature>()
            {
                new()
                {
                    DocumentTypeId = (Guid)company.LegalRepresentativeDocumentTypeId!,
                    FullName = company.LegalRepresentativeFirstName + " " + company.LegalRepresentativeLastName,
                    Email = company.LegalRepresentativeEmail!,
                    IpAddress = _currentUser.IpAddress,
                    Client = _currentUser.Client,
                    Token = company.SignOnboardingToken,
                    SignedDate = signedDate,
                }
            }
        };

        _signedFileRepository.Add(signedFile);

        return signedFile;
    }
}
