namespace ApiTemplate.Domain.Entities.Identity;

public class BlacklistedToken : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedDate { get; set; }

    // Navigation properties
    public User User { get; set; } = default!;
}
