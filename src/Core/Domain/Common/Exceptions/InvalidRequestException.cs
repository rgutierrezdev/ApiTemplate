using System.Net;

namespace ApiTemplate.Domain.Common.Exceptions;

public class InvalidRequestException : CustomException
{
    public InvalidRequestException(string code, string message = "Bad Request", object? data = null)
        : base(code, message, HttpStatusCode.BadRequest, data)
    {
    }
}
