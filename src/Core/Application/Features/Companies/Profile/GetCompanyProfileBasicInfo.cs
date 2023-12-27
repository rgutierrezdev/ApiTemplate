using ApiTemplate.Application.Features.Companies.Dtos;
using ApiTemplate.Application.Features.Companies.Profile.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class GetCompanyProfileBasicInfo
{
    public class Request : IRequest<CompanyProfileBasicInfoDto>
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

    internal class Handler : IRequestHandler<Request, CompanyProfileBasicInfoDto>
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

        public async Task<CompanyProfileBasicInfoDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync<CompanyProfileBasicInfoDto>(
                query => query
                    .Select(c => new CompanyProfileBasicInfoDto()
                    {
                        Current = new CompanyProfileBasicInfoDto.CurrentChange()
                        {
                            LegalType = c.LegalType,
                            LegalName = c.LegalName,
                            CiiuCode = c.CiiuCode,
                            PersonType = c.PersonType,
                            BusinessStructureId = c.BusinessStructureId,
                            BusinessStructureName = c.BusinessStructure!.Name,
                            DocumentTypeId = c.DocumentTypeId,
                            DocumentTypeName = c.DocumentType!.Name,
                            Document = c.Document,
                            VerificationDigit = c.VerificationDigit,
                            CityId = c.CityId,
                            CityName = c.City!.Name,
                            StateId = c.City.StateId,
                            StateName = c.City.State.Name,
                            CountryId = c.City.State.CountryId,
                            CountryName = c.City.State.Country.Name,
                            CountryIsoCode = c.City.State.Country.IsoCode,
                            Address = c.Address,
                            LegalRepresentative = new LegalRepresentativeDto()
                            {
                                FirstName = c.LegalRepresentativeFirstName,
                                LastName = c.LegalRepresentativeLastName,
                                Email = c.LegalRepresentativeEmail,
                                DocumentTypeId = c.LegalRepresentativeDocumentTypeId,
                                DocumentTypeName = c.LegalRepresentativeDocumentType!.Name,
                                Document = c.LegalRepresentativeDocument,
                            },
                            CompanyInfoReviewStatus = c.CompanyInfoReviewStatus,
                            CompanyInfoReviewMessage = c.CompanyInfoReviewMessage,
                        },
                        Change = new CompanyProfileBasicInfoDto.CurrentChange()
                        {
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
                                DocumentTypeName = c.CompanyChange.LegalRepresentativeDocumentType!.Name,
                                Document = c.CompanyChange.LegalRepresentativeDocument,
                            },
                            CompanyInfoReviewStatus = c.CompanyChange.CompanyInfoReviewStatus,
                            CompanyInfoReviewMessage = c.CompanyChange.CompanyInfoReviewMessage,
                        },
                        CompanyInfoReviewStatus = c.CompanyChange.CompanyInfoReviewStatus,
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
