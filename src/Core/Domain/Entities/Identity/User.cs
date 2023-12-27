using ApiTemplate.Domain.Entities.Customers;

namespace ApiTemplate.Domain.Entities.Identity;

public class User : AuditableEntity
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Password { get; set; }
    public bool Enabled { get; set; }
    public string? RecoveryCode { get; set; }
    public DateTime? RecoveryExpireDate { get; set; }
    public bool IsAdmin { get; set; }

    // navigation properties
    public ICollection<AuditTrail> AuditTrails { get; set; } = default!;
    public ICollection<BlacklistedToken> BlacklistedTokens { get; set; } = default!;
    public ICollection<LoginAttempt> LoginAttempts { get; set; } = default!;
    public ICollection<UserRole> UserRoles { get; set; } = default!;
    public ICollection<CompanyUser> CompanyUsers { get; set; } = default!;
    public ICollection<Coupon> CreatedCoupons { get; set; } = default!;

    public void ValidateRecoveryCode(string token)
    {
        if (RecoveryExpireDate == null || RecoveryExpireDate <= DateTime.UtcNow)
        {
            throw new InvalidRequestException(
                ErrorCodes.ResetTokenExpired,
                "Provided reset password token has already expired"
            );
        }

        if (!BCrypt.Net.BCrypt.Verify(RecoveryCode, token))
        {
            throw new UnauthorizedException(ErrorCodes.InvalidResetToken, "Provided reset token is invalid");
        }
    }
}
