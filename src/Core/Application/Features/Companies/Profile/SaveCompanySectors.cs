namespace ApiTemplate.Application.Features.Companies.Profile;

public class SaveCompanySectors
{
    public class Request : BaseRequest, IRequest
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest
    {
        public Difference EconomicSectors { get; set; } = default!;
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
            RuleFor(x => x.EconomicSectors)
                .NotNull();

            RuleForEach(x => x.EconomicSectors.Added)
                .NotNull();

            RuleForEach(x => x.EconomicSectors.Removed)
                .NotEmpty();
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

            var company = await _companyRepository.FirstOrDefaultAsync(query => query
                                  .Include(c => c.CompanyEconomicSectors)
                                  .Where(c => c.Id == request.CompanyId),
                              cancellationToken
                          )
                          ?? throw new NotFoundException(
                              ErrorCodes.CompanyNotFound,
                              $"Company with id '{request.CompanyId}' was not found"
                          );

            foreach (var economicSectorId in request.EconomicSectors.Added)
            {
                var economicSectorExists = company.CompanyEconomicSectors
                    .Any(ces => ces.EconomicSectorId == economicSectorId);

                if (economicSectorExists) continue;

                company.CompanyEconomicSectors.Add(new CompanyEconomicSector()
                {
                    EconomicSectorId = economicSectorId,
                });
            }

            foreach (var economicSectorExists in request.EconomicSectors.Removed)
            {
                var economicSector = company.CompanyEconomicSectors
                    .FirstOrDefault(ces => ces.EconomicSectorId == economicSectorExists);

                if (economicSector == null) continue;

                company.CompanyEconomicSectors.Remove(economicSector);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
