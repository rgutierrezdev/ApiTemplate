using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ApiTemplate.Application.Common.Interfaces;
using ApiTemplate.Domain.Common.Exceptions;
using ApiTemplate.Infrastructure.Auth.Jwt;

namespace ApiTemplate.Infrastructure.Common.Services;

public static class CookieNames
{
    public const string AccessToken = $"ApiTemplate{nameof(AccessToken)}";
    public const string RefreshToken = $"ApiTemplate{nameof(RefreshToken)}";
}

public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtSettings _jwtSettings;

    public CookieService(IHttpContextAccessor httpContextAccessor, IOptions<JwtSettings> jwtSettings)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtSettings = jwtSettings.Value;
    }

    private CookieOptions GetAccessTokenCookieOptions()
    {
        return new CookieOptions()
        {
            SameSite = _jwtSettings.SameSiteStrictCookies ? SameSiteMode.Strict : SameSiteMode.None,
            HttpOnly = true,
            Secure = true,
            Path = "/",
        };
    }

    private CookieOptions GetRefreshTokenCookieOptions()
    {
        return new CookieOptions()
        {
            SameSite = _jwtSettings.SameSiteStrictCookies ? SameSiteMode.Strict : SameSiteMode.None,
            HttpOnly = true,
            Secure = true,
            Path = "/api/auth",
        };
    }

    public void AttachTokenCookies(string accessToken, string refreshToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            throw new InvalidRequestException(ErrorCodes.NoHttpContext);
        }

        httpContext.Response
            .Cookies
            .Append(CookieNames.AccessToken, accessToken, GetAccessTokenCookieOptions());

        httpContext.Response
            .Cookies
            .Append(CookieNames.RefreshToken, refreshToken, GetRefreshTokenCookieOptions());
    }

    public void ClearTokenCookies()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            throw new InvalidRequestException(ErrorCodes.NoHttpContext);
        }

        httpContext.Response
            .Cookies
            .Delete(CookieNames.AccessToken, GetAccessTokenCookieOptions());

        httpContext.Response
            .Cookies
            .Delete(CookieNames.RefreshToken, GetRefreshTokenCookieOptions());
    }

    public string GetRefreshToken()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            throw new InvalidRequestException(ErrorCodes.NoHttpContext);
        }

        if (!httpContext.Request.Cookies.ContainsKey(CookieNames.RefreshToken))

        {
            throw new InvalidRequestException(ErrorCodes.MissingRefreshCookie);
        }

        return httpContext.Request.Cookies[CookieNames.RefreshToken]!;
    }
}
