using ApiTemplate.Application.Features.Auth;
using ApiTemplate.Application.Features.Auth.Dtos;

namespace ApiTemplate.UsersAPI.Controllers;

public class AuthController : BaseApiController
{
    [HttpPost("register-company")]
    [AllowAnonymous]
    [OperationErrors(ErrorCodes.DuplicatedEmail)]
    public Task<LoginResponse> RegisterCompanyAsync(
        RegisterCompany.Request request,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(request, cancellationToken);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [OperationErrors(ErrorCodes.LoginFailed)]
    public Task<LoginResponse> LoginAsync(Login.Request request, CancellationToken cancellationToken)
    {
        return Mediator.Send(request, cancellationToken);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public Task<TokensExpirationDto> RefreshAsync(CancellationToken cancellationToken)
    {
        return Mediator.Send(new RefreshTokens.Request(), cancellationToken);
    }


    [HttpPost("logout")]
    [AllowAnonymous]
    public Task LogoutAsync(CancellationToken cancellationToken)
    {
        return Mediator.Send(new Logout.Request(), cancellationToken);
    }

    [HttpPost("reset-password/check")]
    [AllowAnonymous]
    public Task CheckResetPasswordAsync(
        CheckResetPassword.Request request,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(request, cancellationToken);
    }

    [HttpPost("reset-password/validate")]
    [AllowAnonymous]
    [OperationErrors(ErrorCodes.UserNotFound, ErrorCodes.ResetTokenExpired, ErrorCodes.InvalidResetToken)]
    public Task<ValidateResetPassword.UserDto> ValidateResetPassword(
        ValidateResetPassword.Request request,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(request, cancellationToken);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [OperationErrors(ErrorCodes.UserNotFound, ErrorCodes.ResetTokenExpired, ErrorCodes.InvalidResetToken)]
    public Task ResetPassword(
        ResetPassword.Request request,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(request, cancellationToken);
    }

    [HttpPost("current")]
    [OperationErrors(ErrorCodes.UserNotFound)]
    public Task<CurrentResponse> CurrentAsync(CancellationToken cancellationToken)
    {
        return Mediator.Send(new Current.Request(), cancellationToken);
    }
}
