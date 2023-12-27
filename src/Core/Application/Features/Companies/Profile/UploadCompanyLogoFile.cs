using ApiTemplate.Application.Common.Services.Files;
using File = ApiTemplate.Domain.Entities.File;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class UploadCompanyLogoFile
{
    public class Request : BaseRequest, IRequest<string?>
    {
        public Guid CompanyId { get; set; }
    }

    public class FileRequest : BaseFileRequest
    {
    }

    public class BaseRequest
    {
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
                .In(MimeTypes.Images);
        }
    }

    internal class Handler : IRequestHandler<Request, string?>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly ICurrentUser _currentUser;
        private readonly FileService _fileService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsService _utilsService;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            ICurrentUser currentUser,
            FileService fileService,
            IUnitOfWork unitOfWork,
            IUtilsService utilsService
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _currentUser = currentUser;
            _fileService = fileService;
            _unitOfWork = unitOfWork;
            _utilsService = utilsService;
        }

        public async Task<string?> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.GetByIdAsync(request.CompanyId, cancellationToken)
                          ?? throw new NotFoundException(
                              ErrorCodes.CompanyNotFound,
                              $"Company with id '{request.CompanyId}' was not found"
                          );

            request.File.Base64 = _utilsService.GenerateThumbnailFromBase64(request.File.Base64, 100, 100);

            File? file = null;

            try
            {
                (file, var fileUrl) = await _fileService.DeleteAndUploadNewAsync(
                    company.LogoFileId,
                    request.File,
                    true,
                    new CompanyImagesFolder(company.Id),
                    cancellationToken
                );

                company.LogoFileId = file.Id;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return fileUrl;
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
