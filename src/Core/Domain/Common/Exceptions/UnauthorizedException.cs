using System.Net;

namespace ApiTemplate.Domain.Common.Exceptions;

public class UnauthorizedException : CustomException
{
    public UnauthorizedException(
        string code,
        string message = "Unauthorized",
        object? data = null,
        Exception? innerException = null
    )
        : base(code, message, HttpStatusCode.Unauthorized, data, innerException)
    {
    }
}
