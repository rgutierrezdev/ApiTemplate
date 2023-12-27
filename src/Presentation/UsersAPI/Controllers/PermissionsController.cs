using ApiTemplate.Application.Features.Permissions;
using ApiTemplate.Application.Features.Permissions.Dtos;

namespace ApiTemplate.UsersAPI.Controllers;

public class PermissionsController : BaseApiController
{
    [HttpGet("")]
    public Task<List<PermissionDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetPermissions.Request(), cancellationToken);
    }
}
