using System.Text.RegularExpressions;
using ApiTemplate.Application.Common.Cache;

namespace ApiTemplate.Infrastructure.Caching;

public class CacheKeyService : ICacheKeyService
{
    public string GetKey(string cacheKey, params string[] paramValues)
    {
        var cacheParams = ExtractParams(cacheKey);

        if (cacheParams.Count < paramValues.Length)
        {
            throw new ArgumentException("Missing params");
        }

        for (var i = cacheParams.Count - 1; i >= 0; i--)
        {
            var cacheParam = cacheParams[i];

            cacheKey = cacheKey.Replace(cacheParam, paramValues[i]);
        }

        return cacheKey;
    }

    private List<string> ExtractParams(string input)
    {
        var extractedStrings = new List<string>();
        var regex = new Regex(@"\[(.*?)\]");
        var matches = regex.Matches(input);

        foreach (Match match in matches)
        {
            extractedStrings.Add(match.Groups[0].Value);
        }

        return extractedStrings;
    }
}
