using System.Reflection;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using ApiTemplate.Infrastructure.Auth.Permissions;

namespace ApiTemplate.Infrastructure.OpenAPI;

public class AuthPermissionAttributeProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var attribute = context.MethodInfo.GetCustomAttribute<AuthPermissionAttribute>();

        if (attribute == null)
            return true;

        var operation = context.OperationDescription.Operation;

        if (!string.IsNullOrWhiteSpace(operation.Description))
            operation.Description += Environment.NewLine + Environment.NewLine;

        operation.Description += $"Permission: `{attribute.PermissionName}`";

        return true;
    }
}
