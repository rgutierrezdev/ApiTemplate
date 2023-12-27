namespace ApiTemplate.Application.Features.Companies.Profile;

public class SendCompanyProfileChangesToReview
{
    public class Request : IRequest<CompanyStatus>
    {
        public Guid CompanyId { get; }

        public Request(Guid companyId) => CompanyId = companyId;
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.CompanyId)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, CompanyStatus>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<CompanyStatus> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync(query => query
                    .Include(c => c.CompanyChange)
                    .Include(c => c.CompanyDocumentsFiles)
                    .Where(c => c.Id == request.CompanyId),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.CompanyId}' was not found"
            );

            var validStatuses = new[] { CompanyStatus.Approved, CompanyStatus.Rejected };

            if (!validStatuses.Contains(company.Status))
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyInvalidStatus,
                    $"This company status is '{company.Status.ToString()}'"
                );
            }

            var changeStatuses = new[]
            {
                company.CompanyChange!.CompanyInfoReviewStatus,
                company.CompanyChange.DocumentsReviewStatus,
                company.CompanyChange.CreditReviewStatus,
                company.CompanyChange.AssociatesReviewStatus,
                company.CompanyChange.BillingTaxesReviewStatus,
            };

            var hasSomethingChanged = changeStatuses.Any(s => s == ReviewStatus.ReadyToReview);
            if (!hasSomethingChanged)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyHasNoChanges,
                    $"There are no changes to send to review"
                );
            }

            if (company.CompanyChange.CompanyInfoReviewStatus == ReviewStatus.ReadyToReview)
            {
                company.CompanyChange.CompanyInfoReviewStatus = ReviewStatus.Reviewing;
            }

            if (company.CompanyChange.DocumentsReviewStatus == ReviewStatus.ReadyToReview)
            {
                company.CompanyChange.DocumentsReviewStatus = ReviewStatus.Reviewing;
            }

            if (company.CompanyChange.CreditReviewStatus == ReviewStatus.ReadyToReview)
            {
                company.CompanyChange.CreditReviewStatus = ReviewStatus.Reviewing;
            }

            if (company.CompanyChange.AssociatesReviewStatus == ReviewStatus.ReadyToReview)
            {
                company.CompanyChange.AssociatesReviewStatus = ReviewStatus.Reviewing;
            }

            if (company.CompanyChange.BillingTaxesReviewStatus == ReviewStatus.ReadyToReview)
            {
                company.CompanyChange.BillingTaxesReviewStatus = ReviewStatus.Reviewing;
            }

            company.Status = CompanyStatus.Reviewing;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return company.Status;
        }
    }
}
