using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace ApiTemplate.Infrastructure.Common;

public sealed class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value == null)
        {
            return null;
        }

        var str = value.ToString();

        return string.IsNullOrEmpty(str)
            ? null
            : Regex.Replace(str, "([a-z])([A-Z])", "$1-$2").ToLower();
    }
}
