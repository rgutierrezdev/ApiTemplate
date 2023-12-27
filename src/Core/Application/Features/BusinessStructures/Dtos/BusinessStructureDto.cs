namespace ApiTemplate.Application.Features.BusinessStructures.Dtos;

public class BusinessStructureDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = default!;
}
