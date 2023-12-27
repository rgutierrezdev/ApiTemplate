namespace ApiTemplate.Application.Features.Companies.Profile;

public class SaveCompanyAbout
{
    public class Request : BaseRequest, IRequest
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest
    {
        public string? AboutUs { get; set; }
        public string? ContactEmail { get; set; }
        public string? WebsiteUrl { get; set; }
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
            RuleFor(x => x.AboutUs)
                .MaximumLength(500);

            RuleFor(x => x.ContactEmail)
                .EmailAddress()
                .MaximumLength(320);

            RuleFor(x => x.WebsiteUrl)
                .MaximumLength(100);
        }
    }

    internal class Handler : IRequestHandler<Request>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            ICurrentUser currentUser,
            IUnitOfWork unitOfWork
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
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

            company.AboutUs = request.AboutUs;
            company.ContactEmail = request.ContactEmail;
            company.WebsiteUrl = request.WebsiteUrl;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
