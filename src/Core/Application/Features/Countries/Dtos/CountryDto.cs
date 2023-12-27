namespace ApiTemplate.Application.Features.Countries.Dtos;

public class CountryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string IsoCode { get; set; } = default!;
}
