using Microsoft.AspNetCore.Authorization;

namespace ApiTemplate.Infrastructure.Auth.Permissions;

public class AuthPermissionAttribute : AuthorizeAttribute
{
    public string PermissionName { get; }

    public AuthPermissionAttribute(string permissionName)
    {
        PermissionName = permissionName;
        Policy = "Permissions." + permissionName;
    }
}
