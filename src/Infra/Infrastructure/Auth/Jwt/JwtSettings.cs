using System.ComponentModel.DataAnnotations;

namespace ApiTemplate.Infrastructure.Auth.Jwt;

public class JwtSettings : IValidatableObject
{
    public string AccessTokenSecret { get; set; } = string.Empty;

    public string RefreshTokenSecret { get; set; } = string.Empty;

    public int AccessTokenExpirationInMinutes { get; set; }

    public int RefreshTokenExpirationInDays { get; set; }

    public bool SameSiteStrictCookies { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(AccessTokenSecret))
        {
            yield return new ValidationResult(
                $"{nameof(JwtSettings)}.{nameof(AccessTokenSecret)} is not configured",
                new[] { nameof(AccessTokenSecret) }
            );
        }

        if (string.IsNullOrEmpty(RefreshTokenSecret))
        {
            yield return new ValidationResult(
                $"{nameof(JwtSettings)}.{nameof(RefreshTokenSecret)} is not configured",
                new[] { nameof(RefreshTokenSecret) }
            );
        }
    }
}
