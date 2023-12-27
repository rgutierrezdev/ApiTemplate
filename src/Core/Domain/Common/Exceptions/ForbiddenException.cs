using System.Net;

namespace ApiTemplate.Domain.Common.Exceptions;

public class ForbiddenException : CustomException
{
    public ForbiddenException(string code, string message = "Forbidden")
        : base(code, message, HttpStatusCode.Forbidden)
    {
    }
}
