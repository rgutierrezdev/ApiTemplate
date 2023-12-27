using Microsoft.AspNetCore.Authorization;
using ApiTemplate.Application.Common.Interfaces;

namespace ApiTemplate.Infrastructure.Auth.Permissions;

internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ICurrentUser _currentUser;

    public PermissionAuthorizationHandler(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement
    )
    {
        if (!_currentUser.IsAuthenticated)
            return;

        var permissionName = requirement.Permission.Replace("Permissions.", "");

        var hasPermission = await _currentUser.HasPermissionAsync(_currentUser.Id, permissionName);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
