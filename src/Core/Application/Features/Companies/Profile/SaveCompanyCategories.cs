namespace ApiTemplate.Application.Features.Companies.Profile;

public class SaveCompanyCategories
{
    public class Request : BaseRequest, IRequest
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest
    {
        public Difference Categories { get; set; } = default!;
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
            RuleFor(x => x.Categories)
                .NotNull();

            RuleForEach(x => x.Categories.Added)
                .NotNull();

            RuleForEach(x => x.Categories.Removed)
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
                                  .Include(c => c.CompanyCategories)
                                  .Where(c => c.Id == request.CompanyId),
                              cancellationToken
                          )
                          ?? throw new NotFoundException(
                              ErrorCodes.CompanyNotFound,
                              $"Company with id '{request.CompanyId}' was not found"
                          );

            foreach (var categoryId in request.Categories.Added)
            {
                var categoryExists = company.CompanyCategories.Any(cc => cc.CategoryId == categoryId);
                if (categoryExists) continue;

                company.CompanyCategories.Add(new CompanyCategory()
                {
                    CategoryId = categoryId,
                });
            }

            foreach (var categoryId in request.Categories.Removed)
            {
                var companyCategory = company.CompanyCategories.FirstOrDefault(cc => cc.CategoryId == categoryId);

                if (companyCategory == null) continue;

                company.CompanyCategories.Remove(companyCategory);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
