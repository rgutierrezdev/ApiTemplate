using ApiTemplate.Application.Features.Companies.Profile.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class GetCompanyProfileContacts
{
    public class Request : IRequest<CompanyProfileContactsDto>
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

    internal class Handler : IRequestHandler<Request, CompanyProfileContactsDto>
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

        public async Task<CompanyProfileContactsDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync<CompanyProfileContactsDto>(query => query
                    .Select(c => new CompanyProfileContactsDto()
                    {
                        CompanyInfoReviewStatus = c.CompanyChange!.CompanyInfoReviewStatus,
                        BillingTaxesReviewStatus = c.CompanyChange.BillingTaxesReviewStatus,
                        AssociatesReviewStatus = c.CompanyChange.AssociatesReviewStatus,
                        DocumentsReviewStatus = c.CompanyChange.DocumentsReviewStatus,
                        CreditReviewStatus = c.CompanyChange.CreditReviewStatus,
                    })
                    .Where(c => c.Id == request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.Id}' was not found"
            );

            company.Current = await _companyService.GetContactsAsync(request.Id, cancellationToken);

            return company;
        }
    }
}
