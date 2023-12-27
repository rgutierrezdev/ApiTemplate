using System.Security.Cryptography;
using System.Text;
using ApiTemplate.Application.Common.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ApiTemplate.Infrastructure.Common.Services;

public class UtilsService : IUtilsService
{
    public string GenerateRandomPassword(int length = 20)
    {
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var sb = new StringBuilder();
            var random = new Random();

            for (var i = 0; i < length; i++)
            {
                var index = random.Next(validChars.Length);
                sb.Append(validChars[index]);
            }

            return sb.ToString();
        }
    }

    public string GetFileHash(byte[] file)
    {
        var hash = SHA256.HashData(file);
        var hashString = new StringBuilder();

        foreach (var x in hash)
        {
            hashString.Append($"{x:x2}");
        }

        return hashString.ToString();
    }

    public string GenerateThumbnailFromBase64(string base64Image, int width, int height)
    {
        var imageBytes = Convert.FromBase64String(base64Image);
        using var image = Image.Load(imageBytes);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(width, height)
        }));

        using var resultStream = new MemoryStream();
        image.SaveAsPng(resultStream);

        var thumbnailBase64 = Convert.ToBase64String(resultStream.ToArray());

        return thumbnailBase64;
    }

    public bool CompareProperties<T1, T2>(T1 objectA, T2 objectB, IEnumerable<string> propsToCompare)
    {
        foreach (var propertyName in propsToCompare)
        {
            var propA = typeof(T1).GetProperty(propertyName);
            var propB = typeof(T2).GetProperty(propertyName);

            if (propA == null || propB == null) continue;

            var valueA = propA.GetValue(objectA);
            var valueB = propB.GetValue(objectB);

            if (valueA != null && valueB != null)
            {
                if (!valueA.Equals(valueB))
                {
                    return false;
                }
            }
            else if (valueA != valueB) // one of them is null
            {
                return false;
            }
        }

        return true;
    }

    public void AssignProperties<T1, T2>(T1 sourceObject, T2 targetObject, IEnumerable<string> propsToAssign)
    {
        foreach (var propertyName in propsToAssign)
        {
            var sourceProperty = typeof(T1).GetProperty(propertyName);
            var targetProperty = typeof(T2).GetProperty(propertyName);

            if (sourceProperty == null || targetProperty == null ||
                !targetProperty.CanWrite ||
                !targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                continue;

            var value = sourceProperty.GetValue(sourceObject);
            targetProperty.SetValue(targetObject, value);
        }
    }

    public void AssignNullValues<T>(T targetObject, IEnumerable<string> propsToSetNull)
    {
        foreach (var propertyName in propsToSetNull)
        {
            var targetProperty = typeof(T).GetProperty(propertyName);

            if (targetProperty != null && targetProperty.CanWrite)
            {
                targetProperty.SetValue(targetObject, null);
            }
        }
    }
}
