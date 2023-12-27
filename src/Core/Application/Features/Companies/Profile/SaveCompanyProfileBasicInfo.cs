using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class SaveCompanyProfileBasicInfo
{
    public class Request : BaseRequest, IRequest
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest : BaseSaveCompanyBasicInfo.Request
    {
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
            Include(new BaseSaveCompanyBasicInfo.Validator());
        }
    }

    internal class Handler : IRequestHandler<Request>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IUtilsService _utilsService;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            IUtilsService utilsService
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _utilsService = utilsService;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync(query => query
                    .Include(c => c.CompanyChange)
                    .Where(c => c.Id == request.CompanyId),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.CompanyId}' was not found"
            );

            if (company.Status == CompanyStatus.Reviewing ||
                company.CompanyChange?.CompanyInfoReviewStatus == ReviewStatus.Reviewing)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyInReview,
                    $"This Company status is {ReviewStatus.Reviewing.ToString()}"
                );
            }

            var temp = new CompanyChange
            {
                LegalType = request.LegalType,
                LegalName = request.LegalName,
                CiiuCode = request.CiiuCode,
                PersonType = request.PersonType,
                BusinessStructureId = request.BusinessStructureId,
                DocumentTypeId = request.DocumentTypeId,
                Document = request.Document,
                VerificationDigit = request.VerificationDigit,
                CityId = request.CityId,
                Address = request.Address
            };

            if (temp.PersonType == PersonType.Legal)
            {
                temp.LegalRepresentativeFirstName = request.LegalRepresentative!.FirstName;
                temp.LegalRepresentativeLastName = request.LegalRepresentative.LastName;
                temp.LegalRepresentativeEmail = request.LegalRepresentative.Email;
                temp.LegalRepresentativeDocumentTypeId = request.LegalRepresentative.DocumentTypeId;
                temp.LegalRepresentativeDocument = request.LegalRepresentative.Document;
            }
            else
            {
                temp.LegalRepresentativeFirstName = null;
                temp.LegalRepresentativeLastName = null;
                temp.LegalRepresentativeEmail = null;
                temp.LegalRepresentativeDocumentTypeId = null;
                temp.LegalRepresentativeDocument = null;
            }

            var props = new[]
            {
                nameof(CompanyChange.LegalType),
                nameof(CompanyChange.LegalName),
                nameof(CompanyChange.CiiuCode),
                nameof(CompanyChange.PersonType),
                nameof(CompanyChange.BusinessStructureId),
                nameof(CompanyChange.DocumentTypeId),
                nameof(CompanyChange.Document),
                nameof(CompanyChange.VerificationDigit),
                nameof(CompanyChange.CityId),
                nameof(CompanyChange.Address),
                nameof(CompanyChange.LegalRepresentativeFirstName),
                nameof(CompanyChange.LegalRepresentativeLastName),
                nameof(CompanyChange.LegalRepresentativeEmail),
                nameof(CompanyChange.LegalRepresentativeDocumentTypeId),
                nameof(CompanyChange.LegalRepresentativeDocument),
            };

            var change = company.InitChange();

            var hasSameValues = _utilsService.CompareProperties(company, temp, props);
            if (hasSameValues)
            {
                _utilsService.AssignNullValues(change, props);
                change.CompanyInfoReviewStatus = null;
            }
            else
            {
                _utilsService.AssignProperties(temp, change, props);
                change.CompanyInfoReviewStatus = ReviewStatus.ReadyToReview;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
