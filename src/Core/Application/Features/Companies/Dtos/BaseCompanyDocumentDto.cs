namespace ApiTemplate.Application.Features.Companies.Dtos;

public class BaseCompanyDocumentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public short MinQuantity { get; set; }
    public short MaxQuantity { get; set; }
}

public class BaseCompanyDocumentFileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}
