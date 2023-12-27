namespace ApiTemplate.Application.Features.Companies.Profile;

public class SaveCompanyAddress
{
    public class Request : BaseRequest, IRequest<Guid>
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = default!;
        public Guid CityId { get; set; }
        public string Address { get; set; } = default!;
        public string? AdditionalInfo { get; set; }
        public bool Enabled { get; set; }
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
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.CityId)
                .NotEmpty();

            RuleFor(x => x.Address)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.AdditionalInfo)
                .MaximumLength(300);
        }
    }

    internal class Handler : IRequestHandler<Request, Guid>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<CompanyAddress> _companyAddressRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            IRepository<CompanyAddress> companyAddressRepository,
            ICurrentUser currentUser,
            IUnitOfWork unitOfWork
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _companyAddressRepository = companyAddressRepository;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var companyExists = await _companyRepository.AnyAsync(query => query
                    .Where(c => c.Id == request.CompanyId),
                cancellationToken
            );

            if (!companyExists)
            {
                throw new NotFoundException(
                    ErrorCodes.CompanyNotFound,
                    $"Company with id '{request.CompanyId}' was not found"
                );
            }

            CompanyAddress companyAddress;

            if (request.Id == null)
            {
                companyAddress = new CompanyAddress()
                {
                    Id = Ulid.NewGuid(),
                    CompanyId = request.CompanyId,
                };

                _companyAddressRepository.Add(companyAddress);
            }
            else
            {
                companyAddress = await _companyAddressRepository.GetByIdAsync((Guid)request.Id, cancellationToken)
                                 ?? throw new NotFoundException(
                                     ErrorCodes.CompanyAddressNotFound,
                                     $"Company Address with id '{request.Id}' was not found"
                                 );
            }

            companyAddress.Name = request.Name;
            companyAddress.CityId = request.CityId;
            companyAddress.Address = request.Address;
            companyAddress.AdditionalInfo = request.AdditionalInfo;
            companyAddress.Enabled = request.Enabled;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return companyAddress.Id;
        }
    }
}
