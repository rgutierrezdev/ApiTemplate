using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Domain.Entities;

public class CompanyUserPermission : AuditableWeakEntity
{
    public Guid CompanyUserId { get; set; }
    public Guid PermissionId { get; set; }

    // navigation properties
    public CompanyUser CompanyUser { get; set; } = default!;
    public Permission Permission { get; set; } = default!;
}
