using ApiTemplate.Application.Features.Companies.Onboarding.Dtos;

namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class GetCompanySummary
{
    public class Request : IRequest<CompanySummaryDto>
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

    internal class Handler : IRequestHandler<Request, CompanySummaryDto>
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

        public async Task<CompanySummaryDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync<CompanySummaryDto>(query => query
                    .Select(c => new CompanySummaryDto()
                    {
                        LegalRepresentativeFirstName = c.CompanyChange!.LegalRepresentativeFirstName!,
                        LegalRepresentativeLastName = c.CompanyChange.LegalRepresentativeFirstName!,
                        LegalRepresentativeEmail = c.CompanyChange.LegalRepresentativeFirstName!,
                        LegalRepresentativeDocumentTypeShortName = c.CompanyChange.LegalRepresentativeFirstName!,
                        LegalRepresentativeDocumentTypeName = c.CompanyChange.LegalRepresentativeFirstName!,
                        LegalRepresentativeDocument = c.CompanyChange.LegalRepresentativeFirstName!,
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
