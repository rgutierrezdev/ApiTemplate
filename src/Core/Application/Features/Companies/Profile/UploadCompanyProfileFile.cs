using ApiTemplate.Application.Common.Services.Files;
using ApiTemplate.Application.Features.Companies.Profile.Dtos;
using File = ApiTemplate.Domain.Entities.File;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class UploadCompanyProfileFile
{
    public class Request : BaseRequest, IRequest<CompanyProfileDocumentFileDto.CurrentChange>
    {
        public Guid CompanyId { get; set; }
    }

    public class FileRequest : BaseFileRequest
    {
    }

    public class BaseRequest
    {
        public Guid CompanyDocumentId { get; set; }
        public Guid? ChangeDocumentFileId { get; set; }
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

    internal class Handler : IRequestHandler<Request, CompanyProfileDocumentFileDto.CurrentChange>
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

        public async Task<CompanyProfileDocumentFileDto.CurrentChange> Handle(Request request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync(
                query => query
                    .Include(c => c.CompanyChange)
                    .Include(c => c.CompanyDocumentsFiles)
                    .Where(c => c.Id == request.CompanyId),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.CompanyId}' was not found"
            );

            var companyDocument = await _companyDocumentRepository.GetByIdAsync(request.CompanyDocumentId,
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyDocumentNotFound,
                $"Company Document with id '{request.CompanyDocumentId}' was not found"
            );

            var isUploadingCreditDocument = companyDocument.CreditEnabled == true;

            var statusToValidate = isUploadingCreditDocument
                ? company.CompanyChange?.DocumentsReviewStatus
                : company.CompanyChange?.CreditReviewStatus;

            if (company.Status == CompanyStatus.Reviewing || statusToValidate == ReviewStatus.Reviewing)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyInReview,
                    $"This Company status is {ReviewStatus.Reviewing.ToString()}"
                );
            }

            var isReplacingDocument = isUploadingCreditDocument && request.ChangeDocumentFileId != null;
            if (!isReplacingDocument && isUploadingCreditDocument)
            {
                var currentDocumentFiles = company.CompanyDocumentsFiles
                    .Count(cdf => cdf.CompanyDocumentId == companyDocument.Id);

                if (currentDocumentFiles >= companyDocument.MaxQuantity)
                {
                    throw new InvalidRequestException(
                        ErrorCodes.MaxCompanyDocumentFiles,
                        $"You cannot upload more documents of the given type"
                    );
                }
            }

            File? file = null;

            try
            {
                CompanyDocumentFile companyDocumentFile;

                if (isReplacingDocument)
                {
                    (file, _) = await _fileService.ReplaceAsync(
                        (Guid)request.ChangeDocumentFileId!,
                        request.File,
                        cancellationToken
                    );

                    companyDocumentFile = company.CompanyDocumentsFiles.First(cdf =>
                        cdf.Id == request.ChangeDocumentFileId
                    );
                }
                else
                {
                    (file, _) = await _fileService.UploadNewAsync(
                        request.File,
                        false,
                        new CompanyDocumentsFolder(company.Id),
                        cancellationToken
                    );

                    companyDocumentFile = new CompanyDocumentFile()
                    {
                        Id = file.Id,
                        CompanyDocumentId = request.CompanyDocumentId,
                        ChangeCompanyDocumentFileId = request.ChangeDocumentFileId,
                    };

                    company.CompanyDocumentsFiles.Add(companyDocumentFile);
                }

                companyDocumentFile.Status = ReviewStatus.ReadyToReview;

                var change = company.InitChange();

                if (!isUploadingCreditDocument)
                {
                    change.DocumentsReviewStatus = ReviewStatus.ReadyToReview;
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new CompanyProfileDocumentFileDto.CurrentChange()
                {
                    Id = companyDocumentFile.Id,
                    Name = request.File.Name,
                    Status = companyDocumentFile.Status,
                    ReviewMessage = companyDocumentFile.ReviewMessage
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
