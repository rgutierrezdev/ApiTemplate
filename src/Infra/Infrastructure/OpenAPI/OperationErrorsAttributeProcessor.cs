using System.Reflection;
using System.Text;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using ApiTemplate.Application.Common.Extensions;

namespace ApiTemplate.Infrastructure.OpenAPI;

public class OperationErrorsAttributeProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var attribute = context.MethodInfo.GetCustomAttribute<OperationErrorsAttribute>();

        if (attribute == null || attribute.ErrorCodes.Length == 0)
            return true;

        var operation = context.OperationDescription.Operation;

        if (!string.IsNullOrWhiteSpace(operation.Description))
            operation.Description += Environment.NewLine + Environment.NewLine;

        operation.Description += GetDescription(attribute.ErrorCodes);

        return true;
    }

    private static string GetDescription(IEnumerable<string> errorCodes)
    {
        var sb = new StringBuilder();

        sb.Append("Error codes: ");

        foreach (var errorCode in errorCodes)
        {
            sb.Append($"`{errorCode.ToUnderScoreCase()}` ");
        }

        return sb.ToString();
    }
}
