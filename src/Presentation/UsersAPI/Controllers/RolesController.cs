using ApiTemplate.Application.Features.Roles;
using ApiTemplate.Application.Features.Roles.Dtos;

namespace ApiTemplate.UsersAPI.Controllers;

public class RolesController : BaseApiController
{
    [HttpGet("")]
    public Task<List<RoleDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetAllRoles.Request(), cancellationToken);
    }

    [HttpGet("page")]
    [AuthPermission(Permissions.RolesRead)]
    public Task<PaginationResponse<RoleDto>> PageAsync(
        [FromQuery] GetRolesPage.Request request,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(request, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    [AuthPermission(Permissions.RolesRead)]
    [OperationErrors(ErrorCodes.RoleNotFound)]
    public Task<RolePermissionsDto> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetRole.Request(id), cancellationToken);
    }

    [HttpPost("")]
    [AuthPermission(Permissions.RolesWrite)]
    [OperationErrors(ErrorCodes.RoleNotFound)]
    public Task<Guid> SaveAsync(SaveRole.Request request, CancellationToken cancellationToken
    )
    {
        return Mediator.Send(request, cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    [AuthPermission(Permissions.RolesWrite)]
    [OperationErrors(ErrorCodes.RoleNotFound)]
    public Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return Mediator.Send(new DeleteRole.Request(id), cancellationToken);
    }

    [HttpGet("{id:guid}/users")]
    [AuthPermission(Permissions.RolesRead)]
    public Task<List<RoleUserDto>> GetUsersAsync(Guid id, CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetRoleUsers.Request(id), cancellationToken);
    }
}
