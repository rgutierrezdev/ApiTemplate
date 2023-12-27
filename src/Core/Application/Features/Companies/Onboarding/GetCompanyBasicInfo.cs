using ApiTemplate.Application.Features.Companies.Dtos;
using ApiTemplate.Application.Features.Companies.Onboarding.Dtos;
using ApiTemplate.Domain.Entities.Customers;

namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class GetCompanyBasicInfo
{
    public class Request : IRequest<CompanyBasicInfoDto>
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

    internal class Handler : IRequestHandler<Request, CompanyBasicInfoDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<CouponCompanyUser> _couponCompanyUserRepository;
        private readonly ICurrentUser _currentUser;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            IRepository<CouponCompanyUser> couponCompanyUserRepository,
            ICurrentUser currentUser
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _couponCompanyUserRepository = couponCompanyUserRepository;
            _currentUser = currentUser;
        }

        public async Task<CompanyBasicInfoDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync<CompanyBasicInfoDto>(
                query => query
                    .Select(c => new CompanyBasicInfoDto()
                    {
                        Type = c.Type,
                        LegalType = c.CompanyChange!.LegalType,
                        LegalName = c.CompanyChange.LegalName,
                        CiiuCode = c.CompanyChange.CiiuCode,
                        PersonType = c.CompanyChange.PersonType,
                        BusinessStructureId = c.CompanyChange.BusinessStructureId,
                        BusinessStructureName = c.CompanyChange.BusinessStructure!.Name,
                        DocumentTypeId = c.CompanyChange.DocumentTypeId,
                        DocumentTypeName = c.CompanyChange.DocumentType!.Name,
                        Document = c.CompanyChange.Document,
                        VerificationDigit = c.CompanyChange.VerificationDigit,
                        CityId = c.CompanyChange.CityId,
                        CityName = c.CompanyChange.City!.Name,
                        StateId = c.CompanyChange.City.StateId,
                        StateName = c.CompanyChange.City.State.Name,
                        CountryId = c.CompanyChange.City.State.CountryId,
                        CountryName = c.CompanyChange.City.State.Country.Name,
                        CountryIsoCode = c.CompanyChange.City.State.Country.IsoCode,
                        Address = c.CompanyChange.Address,
                        LegalRepresentative = new LegalRepresentativeDto()
                        {
                            FirstName = c.CompanyChange.LegalRepresentativeFirstName,
                            LastName = c.CompanyChange.LegalRepresentativeLastName,
                            Email = c.CompanyChange.LegalRepresentativeEmail,
                            DocumentTypeId = c.CompanyChange.LegalRepresentativeDocumentTypeId,
                            Document = c.CompanyChange.LegalRepresentativeDocument,
                            DocumentTypeName = c.CompanyChange.LegalRepresentativeDocumentType!.Name
                        },
                    })
                    .Where(c => c.Id == request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.Id}' was not found"
            );

            company.Coupon = await _couponCompanyUserRepository.FirstOrDefaultAsync<CompanyCouponDto>(query => query
                    .Select(ccu => new CompanyCouponDto
                    {
                        Id = ccu.Coupon.Id,
                        Code = ccu.Coupon.Code,
                        Duration = ccu.Coupon.Duration,
                        DiscountPercent = ccu.Coupon.DiscountPercent
                    })
                    .Where(ccu => ccu.CompanyUser.CompanyId == request.Id),
                cancellationToken
            );

            return company;
        }
    }
}
