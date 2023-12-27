using System.Net;

namespace ApiTemplate.Domain.Common.Exceptions;

public abstract class CustomException : Exception
{
    public string Code { get; set; }
    public HttpStatusCode StatusCode { get; }
    public object? CustomData { get; }

    protected CustomException(
        string code,
        string message,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
        object? data = null,
        Exception? innerException = null
    )
        : base(message, innerException)
    {
        Code = code;
        StatusCode = statusCode;
        CustomData = data;
    }
}
