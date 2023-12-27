namespace ApiTemplate.Infrastructure.Mailing;

public class MailSettings
{
    public string BaseUrl { get; set; } = default!;
    public string Domain { get; set; } = default!;
    public string Apikey { get; set; } = default!;
    public bool TestMode { get; set; }
    public string? FromAddress { get; set; }
    public string? FromName { get; set; }
}
