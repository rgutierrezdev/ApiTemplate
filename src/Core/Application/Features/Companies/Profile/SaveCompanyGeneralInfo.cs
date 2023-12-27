namespace ApiTemplate.Application.Features.Companies.Profile;

public class SaveCompanyGeneralInfo
{
    public class Request : BaseRequest, IRequest
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest
    {
        public EmployeesNumber? EmployeesNumber { get; set; }
        public YearlyIncome? YearlyIncome { get; set; }
        public DateOnly? ConstitutionDate { get; set; }
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

            company.EmployeesNumber = request.EmployeesNumber;
            company.YearlyIncome = request.YearlyIncome;
            company.ConstitutionDate = request.ConstitutionDate;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
