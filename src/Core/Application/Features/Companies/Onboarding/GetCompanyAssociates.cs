using ApiTemplate.Application.Features.Companies.Dtos;
using ApiTemplate.Application.Features.Companies.Onboarding.Dtos;

namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class GetCompanyAssociates
{
    public class Request : IRequest<CompanyAssociatesDto>
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

    internal class Handler : IRequestHandler<Request, CompanyAssociatesDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly ICurrentUser _currentUser;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            ICurrentUser currentUser
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _currentUser = currentUser;
        }

        public async Task<CompanyAssociatesDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync<CompanyAssociatesDto>(query => query
                    .Select(c => new CompanyAssociatesDto()
                    {
                        HasPepRelative = c.CompanyChange!.HasPepRelative,
                        UnderOath = c.CompanyChange.UnderOath,
                        Associates = c.CompanyAssociates
                            .Where(ca => ca.IsChange)
                            .Select(ca => new CompanyAssociateDto(
                                ca.Id,
                                ca.Name,
                                ca.DocumentTypeId,
                                ca.DocumentType.ShortName,
                                ca.DocumentType.Name,
                                ca.Document,
                                ca.ParticipationPercent,
                                ca.Pep
                            ))
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
