namespace ApiTemplate.Application.Common.Mailing;

public class BatchMailRequest : MailRequest
{
    public IDictionary<string, IDictionary<string, object>> To { get; set; } = default!;
}
