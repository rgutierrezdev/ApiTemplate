using ApiTemplate.Application.Features.Users;

namespace ApiTemplate.UsersAPI.Controllers;

public class UsersController : BaseApiController
{
    [HttpGet("page")]
    [AuthPermission(Permissions.UsersRead)]
    public Task<PaginationResponse<GetUsersPage.Response>> PageAsync(
        [FromQuery] GetUsersPage.Request request,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(request, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    [AuthPermission(Permissions.UsersRead)]
    [OperationErrors(ErrorCodes.UserNotFound)]
    public Task<GetUser.Response> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetUser.Request(id), cancellationToken);
    }

    [HttpPost("")]
    [AuthPermission(Permissions.UsersWrite)]
    [OperationErrors(ErrorCodes.UserNotFound, ErrorCodes.DuplicatedEmail)]
    public Task<SaveUser.Response> SaveAsync(
        SaveUser.Request request,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(request, cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    [AuthPermission(Permissions.UsersWrite)]
    [OperationErrors(ErrorCodes.UserNotFound)]
    public Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return Mediator.Send(new DeleteUser.Request(id), cancellationToken);
    }
}
