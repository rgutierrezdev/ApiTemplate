namespace ApiTemplate.Application.Features.Permissions.Dtos;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string? Description { get; set; }
}
