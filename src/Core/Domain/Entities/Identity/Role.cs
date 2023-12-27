namespace ApiTemplate.Domain.Entities.Identity;

public class Role : AuditableEntity
{
    public string Name { get; set; } = default!;

    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; set; } = default!;
    public ICollection<UserRole> UserRoles { get; set; } = default!;
}
