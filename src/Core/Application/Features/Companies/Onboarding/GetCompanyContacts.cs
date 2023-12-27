using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class GetCompanyContacts
{
    public class Request : IRequest<BaseCompanyContactsDto>
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

    internal class Handler : IRequestHandler<Request, BaseCompanyContactsDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Company> _companyRepository;
        private readonly CompanyService _companyService;

        public Handler(
            IValidator<Request> validator,
            ICurrentUser currentUser,
            IRepository<Company> companyRepository,
            CompanyService companyService
        )
        {
            _validator = validator;
            _currentUser = currentUser;
            _companyRepository = companyRepository;
            _companyService = companyService;
        }

        public async Task<BaseCompanyContactsDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var companyExists = await _companyRepository.AnyAsync(
                query => query.Where(c => c.Id == request.Id),
                cancellationToken
            );

            if (!companyExists)
            {
                throw new NotFoundException(
                    ErrorCodes.CompanyNotFound,
                    $"Company with id '{request.Id}' was not found"
                );
            }

            return await _companyService.GetContactsAsync(request.Id, cancellationToken);
        }
    }
}
