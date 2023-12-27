using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class SaveCompanyProfileBillingTaxes
{
    public class Request : BaseRequest, IRequest
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest : BaseSaveCompanyBillingTaxes.Request
    {
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.CompanyId)
                .NotEmpty();

            Include(new BaseValidator());
        }
    }

    internal class BaseValidator : AbstractValidator<BaseRequest>
    {
        public BaseValidator()
        {
            Include(new BaseSaveCompanyBillingTaxes.Validator());
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
                company.CompanyChange?.BillingTaxesReviewStatus == ReviewStatus.Reviewing)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyInReview,
                    $"This Company status is {ReviewStatus.Reviewing.ToString()}"
                );
            }

            var temp = new CompanyChange
            {
                RetentionSubject = request.RetentionSubject,
                RequiredToDeclareIncome = request.RequiredToDeclareIncome,
                VatResponsible = request.VatResponsible,
                DianGreatContributor = request.DianGreatContributor,
                DianGreatContributorRes = request.DianGreatContributor
                    ? request.DianGreatContributorRes
                    : null,
                SalesRetentionAgent = request.SalesRetentionAgent,
                SalesRetentionAgentRes = request.SalesRetentionAgent
                    ? request.SalesRetentionAgentRes
                    : null,
                IncomeSelfRetainer = request.IncomeSelfRetainer,
                IncomeSelfRetainerRes = request.IncomeSelfRetainer
                    ? request.IncomeSelfRetainerRes
                    : null,
                Regime = request.Regime,
                IcaActivity = request.IcaActivity,
                IcaAutoRetainer = request.IcaAutoRetainer,
                IcaAutoRetainerRes = request.IcaAutoRetainer
                    ? request.IcaAutoRetainerRes
                    : null
            };

            var props = new[]
            {
                nameof(CompanyChange.RetentionSubject),
                nameof(CompanyChange.RequiredToDeclareIncome),
                nameof(CompanyChange.VatResponsible),
                nameof(CompanyChange.DianGreatContributor),
                nameof(CompanyChange.DianGreatContributorRes),
                nameof(CompanyChange.SalesRetentionAgent),
                nameof(CompanyChange.SalesRetentionAgentRes),
                nameof(CompanyChange.IncomeSelfRetainer),
                nameof(CompanyChange.IncomeSelfRetainerRes),
                nameof(CompanyChange.Regime),
                nameof(CompanyChange.IcaActivity),
                nameof(CompanyChange.IcaAutoRetainer),
                nameof(CompanyChange.IcaAutoRetainerRes),
            };

            var change = company.InitChange();

            var hasSameValues = _utilsService.CompareProperties(company, temp, props);
            if (hasSameValues)
            {
                _utilsService.AssignNullValues(change, props);
                change.BillingTaxesReviewStatus = null;
            }
            else
            {
                _utilsService.AssignProperties(temp, change, props);
                change.BillingTaxesReviewStatus = ReviewStatus.ReadyToReview;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
