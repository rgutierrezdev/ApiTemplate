namespace ApiTemplate.Infrastructure.Exceptions;

public class ExceptionError
{
    public string? Message { get; set; }
    public object? Data { get; set; }
    public string? Source { get; set; }
    public string[]? StackTrace { get; set; }
    public ExceptionError? InnerError { get; set; }
}

public class ErrorResult : ExceptionError
{
    public string Code { get; set; } = default!;
    public int StatusCode { get; set; }
}
