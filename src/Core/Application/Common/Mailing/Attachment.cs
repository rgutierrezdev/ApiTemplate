namespace ApiTemplate.Application.Common.Mailing;

public enum AttachmentType
{
    Base64,
    Filepath
}

public class Attachment
{
    public Attachment(AttachmentType type, string content, string name)
    {
        Type = type;
        Content = content;
        Name = name;
    }

    public AttachmentType Type { get; set; }
    public string Content { get; set; }
    public string Name { get; set; }
}
