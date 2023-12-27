namespace ApiTemplate.Application.Common.Mailing;

public abstract class MailRequest
{
    public string Template { get; set; } = default!;

    public string Subject { get; set; } = default!;

    public string? ReplyToAddress { get; set; }

    public string? FromAddress { get; set; }

    public string? FromName { get; set; }

    public IEnumerable<string>? Tags { get; set; }

    public IEnumerable<Attachment>? Attachments { get; set; }
}
