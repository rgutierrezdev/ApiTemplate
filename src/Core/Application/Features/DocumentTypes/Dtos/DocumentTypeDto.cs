namespace ApiTemplate.Application.Features.DocumentTypes.Dtos;

public class DocumentTypeDto
{
    public Guid Id { get; set; }
    public string ShortName { get; set; } = default!;
    public string Name { get; set; } = default!;
}
