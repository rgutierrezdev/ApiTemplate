using ApiTemplate.Application.Common.Services.Files;

namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class DeleteCompanyFile
{
    public class Request : IRequest<Guid>
    {
        public Guid CompanyId { get; }
        public Guid DocumentFileId { get; }

        public Request(Guid companyId, Guid documentFileId)
        {
            CompanyId = companyId;
            DocumentFileId = documentFileId;
        }
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty();

            RuleFor(x => x.DocumentFileId)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, Guid>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<CompanyDocumentFile> _companyDocumentFileRepository;
        private readonly ICurrentUser _currentUser;
        private readonly FileService _fileService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            IRepository<CompanyDocumentFile> companyDocumentFileRepository,
            ICurrentUser currentUser,
            FileService fileService,
            IUnitOfWork unitOfWork
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _companyDocumentFileRepository = companyDocumentFileRepository;
            _currentUser = currentUser;
            _fileService = fileService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var companyStatus = await _companyRepository.FirstOrDefaultAsync<CompanyStatus>(query => query
                    .Select(c => c.Status)
                    .Where(c => c.Id == request.CompanyId),
                cancellationToken
            );

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var companyDocFile = await _companyDocumentFileRepository.GetByIdAsync(
                request.DocumentFileId,
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyDocumentFileNotFound,
                $"Company document file with id '{request.DocumentFileId}' was not found"
            );

            if (companyDocFile.CompanyId != request.CompanyId)
            {
                throw new InvalidRequestException(ErrorCodes.ParamsMissMatch);
            }

            if (companyStatus != CompanyStatus.OnBoarding)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyInvalidStatus,
                    $"This company status is '{companyStatus.ToString()}'"
                );
            }

            _companyDocumentFileRepository.Delete(companyDocFile);

            await _fileService.DeleteAsync(companyDocFile.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.DocumentFileId;
        }
    }
}
