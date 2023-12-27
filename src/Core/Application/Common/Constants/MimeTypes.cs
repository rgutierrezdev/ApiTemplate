namespace ApiTemplate.Application.Common.Constants;

public static class MimeTypes
{
    public const string Pdf = "application/pdf";
    public const string Jpeg = "image/jpeg";
    public const string Gif = "image/gif";
    public const string Png = "image/png";
    public const string Svg = "image/svg+xml";
    public const string Bmp = "image/bmp";
    public const string Webp = "image/webp";
    public const string Icon = "image/vnd.microsoft.icon";
    public const string Tiff = "image/tiff";
    public const string Doc = "application/msword";
    public const string Docx = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

    public static readonly string[] Images =
    {
        Jpeg,
        Gif,
        Png,
        // Svg, Removed since it can include JS, so it needs to be sanitized first
        Bmp,
        // Webp, Removed since it can include JS, so it needs to be sanitized first
        Icon,
        Tiff,
    };

    public static readonly string[] Docs =
    {
        Pdf,
        Doc,
        Docx,
    };
}
