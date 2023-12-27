using System.Net;

namespace ApiTemplate.Domain.Common.Exceptions;

public class NotFoundException : CustomException
{
    public NotFoundException(string code, string message = "Record Not Found", object? data = null)
        : base(code, message, HttpStatusCode.NotFound, data)
    {
    }
}
