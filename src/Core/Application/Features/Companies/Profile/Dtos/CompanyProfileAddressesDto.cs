namespace ApiTemplate.Application.Features.Companies.Profile.Dtos;

public class CompanyProfileAddressesDto : BaseProfileResponse
{
    public ICollection<CurrenChange> Current { get; set; } = default!;

    public class CurrenChange
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public Guid CityId { get; set; }
        public string CityName { get; set; } = default!;
        public Guid StateId { get; set; }
        public string StateName { get; set; } = default!;
        public Guid CountryId { get; set; }
        public string CountryName { get; set; } = default!;
        public string CountryIsoCode { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string? AdditionalInfo { get; set; } = default!;
    }
}
