using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ApiTemplate.Domain.Common.Exceptions;
using ApiTemplate.Infrastructure.Common.Services;

namespace ApiTemplate.Infrastructure.Auth.Jwt;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtSettings _jwtSettings;

    public ConfigureJwtBearerOptions(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(string.Empty, options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
            return;

        var secret = Encoding.ASCII.GetBytes(_jwtSettings.AccessTokenSecret);

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secret),
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();

                if (!context.Response.HasStarted)
                {
                    throw new UnauthorizedException(ErrorCodes.InvalidAccessToken);
                }

                return Task.CompletedTask;
            },
            OnForbidden = _ => throw new ForbiddenException(ErrorCodes.NoPermission),
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey(CookieNames.AccessToken))
                {
                    context.Token = context.Request.Cookies[CookieNames.AccessToken];
                }

                return Task.CompletedTask;
            }
        };
    }
}
