using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class SaveCompanyProfileElectronicInvoice
{
    public class Request : BaseRequest, IRequest
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest : BaseSaveElectronicInvoice.Request
    {
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator(IRepository<Company> repository, ICurrentUser currentUser)
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
            Include(new BaseSaveElectronicInvoice.Validator());
        }
    }

    internal class Handler : IRequestHandler<Request>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly CompanyService _companyService;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            CompanyService companyService
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _companyService = companyService;
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

            _companyService.SetElectronicInvoice(company, request);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
