namespace ApiTemplate.Application.Common.Mailing;

public class SimpleMailRequest : MailRequest
{
    public IEnumerable<string> To { get; set; } = default!;

    public IDictionary<string, object>? Vars { get; set; }

    public List<string>? Bcc { get; set; }

    public List<string>? Cc { get; set; }
}
