namespace ApiTemplate.Infrastructure.OpenAPI;

[AttributeUsage(AttributeTargets.Method)]
public class OperationErrorsAttribute : Attribute
{
    public string[] ErrorCodes { get; }

    public OperationErrorsAttribute(params string[] errorCodes)
    {
        ErrorCodes = errorCodes;
    }
}
