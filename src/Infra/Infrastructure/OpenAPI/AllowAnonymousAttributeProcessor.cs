using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace ApiTemplate.Infrastructure.OpenAPI;

public class AllowAnonymousAttributeProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var attribute = context.MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>();

        if (attribute == null)
            return true;

        var operation = context.OperationDescription.Operation;

        if (!string.IsNullOrWhiteSpace(operation.Description))
            operation.Description += Environment.NewLine + Environment.NewLine;

        operation.Description += "`Anonymous Access`";

        return true;
    }
}
