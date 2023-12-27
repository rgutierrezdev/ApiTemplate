namespace ApiTemplate.Application.Common.Models;

public class EncodedFileResponse
{
    public string MimeType { get; }
    public string Base64 { get; }

    public EncodedFileResponse(string mimeType, string base64)
    {
        MimeType = mimeType;
        Base64 = base64;
    }
}
