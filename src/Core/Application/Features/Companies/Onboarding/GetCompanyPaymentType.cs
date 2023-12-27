using ApiTemplate.Application.Features.Companies.Onboarding.Dtos;

namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class GetCompanyPaymentType
{
    public class Request : IRequest<CompanyPaymentTypeDto>
    {
        public Guid Id { get; set; }

        public Request(Guid id)
        {
            Id = id;
        }
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, CompanyPaymentTypeDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly ICurrentUser _currentUser;

        public Handler(IValidator<Request> validator, IRepository<Company> companyRepository, ICurrentUser currentUser)
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _currentUser = currentUser;
        }

        public async Task<CompanyPaymentTypeDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync<CompanyPaymentTypeDto>(query => query
                    .Select(c => new CompanyPaymentTypeDto()
                    {
                        CreditEnabled = c.CompanyChange!.CreditEnabled ?? c.CreditEnabled,
                        AuthorizesFinancialInformation = c.AuthorizesFinancialInformation
                    })
                    .Where(c => c.Id == request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.Id}' was not found"
            );

            return company;
        }
    }
}
