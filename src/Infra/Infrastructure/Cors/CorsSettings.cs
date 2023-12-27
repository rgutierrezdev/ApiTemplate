namespace ApiTemplate.Infrastructure.Cors;

public class CorsSettings
{
    public List<string> Origins { get; set; } = default!;
    public List<string> Methods { get; set; } = default!;
    public List<string> Headers { get; set; } = default!;
}
