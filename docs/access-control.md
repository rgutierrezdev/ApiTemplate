# Access Control

By Default all controller endpoints are required for authentication (User must be logged-in), if you want an endpoint to
not require authentication you can use the `AllowAnonymous` attribute:

```csharp
public class AuthController : BaseApiController
{
    [HttpPost("login")]
    [AllowAnonymous] // <---
    public async Task<Response> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        // your logic
    }
}
```

## Permissions

.NET ApiTemplate APIs use [Role-based access control](https://en.wikipedia.org/wiki/Role-based_access_control) to handle
access to the different API endpoints.

If you want to require an user to have a certain permission to access an endpoint, use the `AuthPermission` attribute
and pass the permission name as a parameter.

All the permissions should be defined in the `src/Application/Common/Constants/Permissions.cs` file in order to
avoid typo issues and easily keep track of where each permission is being used.

```csharp
public class UsersController : BaseApiController
{
    [HttpGet("page")]
    [AuthPermission(Permissions.UsersRead)] // <---
    public Task<Response> SearchAsync([FromQuery] GetUsersPage.Request request, CancellationToken cancellationToken)
    {
        // your logic
    }
}
```
