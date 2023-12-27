using ApiTemplate.Application.Common.Services.Files;
using ApiTemplate.Application.Features.Companies.Dtos;
using File = ApiTemplate.Domain.Entities.File;

namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class UploadCompanyFile
{
    public class Request : BaseRequest, IRequest<BaseCompanyDocumentFileDto>
    {
        public Guid CompanyId { get; set; }
    }

    public class FileRequest : BaseFileRequest
    {
    }

    public class BaseRequest
    {
        public Guid CompanyDocumentId { get; set; }
        public FileRequest File { get; set; } = default!;
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty();

            Include(new BaseValidator());
        }
    }

    internal class BaseValidator : AbstractValidator<BaseRequest>
    {
        public BaseValidator()
        {
            RuleFor(x => x.CompanyDocumentId)
                .NotEmpty();

            RuleFor(x => x.File)
                .NotEmpty()
                .SetValidator(new FileRequestValidator());
        }
    }

    internal class FileRequestValidator : AbstractValidator<FileRequest>
    {
        public FileRequestValidator()
        {
            Include(new BaseFileRequestValidator());

            RuleFor(x => x.Mime)
                .In(MimeTypes.Docs);
        }
    }

    internal class Handler : IRequestHandler<Request, BaseCompanyDocumentFileDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<CompanyDocument> _companyDocumentRepository;
        private readonly ICurrentUser _currentUser;
        private readonly FileService _fileService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            IRepository<CompanyDocument> companyDocumentRepository,
            ICurrentUser currentUser,
            FileService fileService,
            IUnitOfWork unitOfWork
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _companyDocumentRepository = companyDocumentRepository;
            _currentUser = currentUser;
            _fileService = fileService;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseCompanyDocumentFileDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync(
                query => query
                    .Include(c => c.CompanyDocumentsFiles.Where(
                        cdf => cdf.CompanyDocumentId == request.CompanyDocumentId &&
                               cdf.ChangeCompanyDocumentFileId != null
                    ))
                    .Where(c => c.Id == request.CompanyId),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.CompanyId}' was not found"
            );

            if (company.Status != CompanyStatus.OnBoarding)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyInvalidStatus,
                    $"This company status is '{company.Status.ToString()}'"
                );
            }

            var companyDocument = await _companyDocumentRepository
                                      .GetByIdAsync(request.CompanyDocumentId, cancellationToken)
                                  ?? throw new NotFoundException(
                                      ErrorCodes.CompanyDocumentNotFound,
                                      $"Company Document with id '{request.CompanyDocumentId}' was not found"
                                  );

            if (companyDocument.CreditEnabled != null && companyDocument.CreditEnabled != company.CreditEnabled)
            {
                throw new InvalidRequestException(
                    ErrorCodes.InvalidCreditAvailability,
                    "Document not valid for company credit availability"
                );
            }

            var currentDocumentFiles = company.CompanyDocumentsFiles
                .Count(cdf => cdf.CompanyDocumentId == request.CompanyDocumentId);

            if (currentDocumentFiles >= companyDocument.MaxQuantity)
            {
                throw new InvalidRequestException(
                    ErrorCodes.MaxCompanyDocumentFiles,
                    $"You cannot upload more documents of the given type"
                );
            }

            File? file = null;

            try
            {
                (file, _) = await _fileService.UploadNewAsync(
                    request.File,
                    false,
                    new CompanyDocumentsFolder(company.Id),
                    cancellationToken
                );

                var companyDocumentFile = new CompanyDocumentFile()
                {
                    Id = file.Id,
                    CompanyDocumentId = request.CompanyDocumentId,
                    Status = ReviewStatus.Reviewing,
                    ChangeCompanyDocumentFileId = file.Id,
                };

                company.CompanyDocumentsFiles.Add(companyDocumentFile);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new BaseCompanyDocumentFileDto()
                {
                    Id = companyDocumentFile.Id,
                    Name = request.File.Name,
                };
            }
            catch (Exception)
            {
                if (file != null)
                    await _fileService.DeleteAsync(file.Id, cancellationToken);

                throw;
            }
        }
    }
}
