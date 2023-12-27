namespace ApiTemplate.Application.Features.Roles.Dtos;

public class RolePermissionsDto : RoleDto
{
    public List<Guid> Permissions { get; set; } = default!;
}
