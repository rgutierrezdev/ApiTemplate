using ApiTemplate.Application.Features.Companies.Profile.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class GetCompanyProfileAddresses
{
    public class Request : IRequest<CompanyProfileAddressesDto>
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

    internal class Handler : IRequestHandler<Request, CompanyProfileAddressesDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Company> _companyRepository;

        public Handler(
            IValidator<Request> validator,
            ICurrentUser currentUser,
            IRepository<Company> companyRepository
        )
        {
            _validator = validator;
            _currentUser = currentUser;
            _companyRepository = companyRepository;
        }

        public async Task<CompanyProfileAddressesDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync<CompanyProfileAddressesDto>(query => query
                    .Select(c => new CompanyProfileAddressesDto()
                    {
                        Current = c.CompanyAddresses.Select(ca => new CompanyProfileAddressesDto.CurrenChange()
                            {
                                Id = ca.Id,
                                Name = ca.Name,
                                CityId = ca.CityId,
                                CityName = ca.City.Name,
                                StateId = ca.City.StateId,
                                StateName = ca.City.State.Name,
                                CountryId = ca.City.State.CountryId,
                                CountryName = ca.City.State.Country.Name,
                                CountryIsoCode = ca.City.State.Country.IsoCode,
                                Address = ca.Address,
                                AdditionalInfo = ca.AdditionalInfo,
                            })
                            .ToList(),
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

            return company;
        }
    }
}
