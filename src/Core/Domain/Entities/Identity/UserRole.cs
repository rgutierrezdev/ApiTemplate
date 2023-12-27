namespace ApiTemplate.Domain.Entities.Identity;

public class UserRole : AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // navigation rules
    public User User { get; set; } = default!;
    public Role Role { get; set; } = default!;
}
