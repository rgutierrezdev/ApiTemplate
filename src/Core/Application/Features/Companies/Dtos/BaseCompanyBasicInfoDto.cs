namespace ApiTemplate.Application.Features.Companies.Dtos;

public class BaseCompanyBasicInfoDto
{
    public CompanyLegalType? LegalType { get; set; }

    public string? LegalName { get; set; } = default!;
    public string? CiiuCode { get; set; } = default!;
    public PersonType? PersonType { get; set; }
    public Guid? BusinessStructureId { get; set; }
    public string? BusinessStructureName { get; set; }
    public Guid? DocumentTypeId { get; set; }
    public string? DocumentTypeName { get; set; }
    public string? Document { get; set; } = default!;
    public string? VerificationDigit { get; set; } = default!;

    public Guid? CityId { get; set; }
    public string? CityName { get; set; }
    public Guid? StateId { get; set; }
    public string? StateName { get; set; }
    public Guid? CountryId { get; set; }
    public string? CountryName { get; set; }
    public string? CountryIsoCode { get; set; }
    public string? Address { get; set; } = default!;

    public LegalRepresentativeDto? LegalRepresentative { get; set; }
}

public class LegalRepresentativeDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public Guid? DocumentTypeId { get; set; }
    public string? Document { get; set; }
    public string? DocumentTypeName { get; set; }
}
