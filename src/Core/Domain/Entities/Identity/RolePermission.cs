namespace ApiTemplate.Domain.Entities.Identity;

public class RolePermission : AuditableWeakEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    // navigation properties
    public Role Role { get; set; } = default!;
    public Permission Permission { get; set; } = default!;
}
