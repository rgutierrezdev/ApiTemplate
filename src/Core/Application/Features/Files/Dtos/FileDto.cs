namespace ApiTemplate.Application.Features.Files.Dtos;

public class FileDto
{
    public string Name { get; set; } = default!;
    public string Mime { get; set; } = default!;
    public string Base64 { get; set; } = default!;
    public long Size { get; set; }
}
