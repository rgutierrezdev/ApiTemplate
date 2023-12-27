using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using ApiTemplate.Application.Common.Extensions;

namespace ApiTemplate.Infrastructure.OpenAPI;

public class CamelCaseParameterProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        foreach (var operation in context.Document.Operations.Select((o => o.Operation)))
        {
            foreach (var parameter in operation.Parameters)
            {
                var split = parameter.Name
                    .Split('.')
                    .ToList()
                    .Select(split => split.ToCamelCase());

                parameter.Name = string.Join(".", split);
            }
        }
    }
}
