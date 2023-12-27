namespace ApiTemplate.Domain.Entities.Identity;

public enum LoginAttemptStatus
{
    Successful = 1,
    UserNotFound = 2,
    IncorrectPassword = 3,
}

public class LoginAttempt : BaseEntity
{
    public string Email { get; set; } = default!;
    public Guid? UserId { get; set; }
    public LoginAttemptStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }

    // Navigation properties
    public User? User { get; set; }
}
