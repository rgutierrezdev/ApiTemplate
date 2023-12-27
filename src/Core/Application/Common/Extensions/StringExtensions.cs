using System.Text.RegularExpressions;

namespace ApiTemplate.Application.Common.Extensions;

public static class StringExtensions
{
    public static string ToUnderScoreCase(this string camelPascalCaseText)
    {
        if (string.IsNullOrEmpty(camelPascalCaseText))
            return camelPascalCaseText;

        var underscoreCase = Regex.Replace(camelPascalCaseText, "(?<!^)([A-Z])", "_$1");

        return underscoreCase.ToLower();
    }

    public static string ToCamelCase(this string pascalCaseText)
    {
        if (string.IsNullOrEmpty(pascalCaseText))
            return pascalCaseText;

        return char.ToLower(pascalCaseText[0]) + pascalCaseText.Substring(1);
    }
}
