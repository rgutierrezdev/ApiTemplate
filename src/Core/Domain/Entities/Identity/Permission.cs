namespace ApiTemplate.Domain.Entities.Identity;

public class Permission : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsAdmin { get; set; }

    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; set; } = default!;
    public ICollection<CompanyUserPermission> CompanyUserPermissions { get; set; } = default!;
}
