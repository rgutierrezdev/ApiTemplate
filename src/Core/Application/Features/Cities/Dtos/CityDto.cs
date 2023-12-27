namespace ApiTemplate.Application.Features.Cities.Dtos;

public class CityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid StateId { get; set; }
    public string StateName { get; set; } = default!;
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = default!;
}
