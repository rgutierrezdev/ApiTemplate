using ApiTemplate.Application.Common.Services.Files;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class DeleteCompanyProfileFile
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

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync(
                query => query
                    .Include(c => c.CompanyChange)
                    .Where(c => c.Id == request.CompanyId),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.CompanyId}' was not found"
            );

            var companyDocFile = await _companyDocumentFileRepository.FirstOrDefaultAsync(query => query
                    .Include(cdf => cdf.CompanyDocument)
                    .Where(cdf => cdf.Id == request.DocumentFileId),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyDocumentFileNotFound,
                $"Company document file with id '{request.DocumentFileId}' was not found"
            );

            if (companyDocFile.CompanyId != request.CompanyId)
            {
                throw new InvalidRequestException(ErrorCodes.ParamsMissMatch);
            }

            var isDeletingCreditDocument = companyDocFile.CompanyDocument.CreditEnabled == true;

            var statusToValidate = isDeletingCreditDocument
                ? company.CompanyChange?.DocumentsReviewStatus
                : company.CompanyChange?.CreditReviewStatus;

            if (company.Status == CompanyStatus.Reviewing || statusToValidate == ReviewStatus.Reviewing)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyInReview,
                    $"This Company status is {ReviewStatus.Reviewing.ToString()}"
                );
            }

            var change = company.InitChange();

            if (!isDeletingCreditDocument)
            {
                var isThereMoreDocFiles = await _companyDocumentFileRepository.AnyAsync(query => query
                        .Where(cdf => cdf.CompanyId == request.CompanyId && cdf.Id != request.DocumentFileId &&
                                      (cdf.CompanyDocument.CreditEnabled == null ||
                                       cdf.CompanyDocument.CreditEnabled == false)),
                    cancellationToken);

                if (!isThereMoreDocFiles)
                {
                    change.DocumentsReviewStatus = null;
                }
            }

            _companyDocumentFileRepository.Delete(companyDocFile);
            await _fileService.DeleteAsync(companyDocFile.Id, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.DocumentFileId;
        }
    }
}
