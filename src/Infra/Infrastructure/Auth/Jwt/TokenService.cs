using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ApiTemplate.Application.Common.Interfaces;
using ApiTemplate.Application.Common.Models;
using ApiTemplate.Domain.Common;
using ApiTemplate.Domain.Common.Exceptions;

namespace ApiTemplate.Infrastructure.Auth.Jwt;

internal class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public TokenResponse GenerateAuthTokens(Guid userId)
    {
        var tokenId = Ulid.NewGuid().ToString();

        var accessTokenExpiryDate = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes);
        var refreshTokenExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        var accessToken = GenerateToken(
            GetSigningCredentials(_jwtSettings.AccessTokenSecret),
            new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Sid, tokenId),
            },
            accessTokenExpiryDate
        );

        var refreshToken = GenerateToken(
            GetSigningCredentials(_jwtSettings.RefreshTokenSecret),
            new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Sid, tokenId),
            },
            refreshTokenExpiryDate
        );

        return new TokenResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiryDate = accessTokenExpiryDate,
            RefreshTokenExpiryDate = refreshTokenExpiryDate,
        };
    }

    public IEnumerable<Claim> VerifyRefreshToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = Encoding.ASCII.GetBytes(_jwtSettings.RefreshTokenSecret);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secret),
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return jwtToken.Claims;
        }
        catch (Exception e)
        {
            throw new UnauthorizedException(ErrorCodes.InvalidRefreshToken, innerException: e);
        }
    }

    private SigningCredentials GetSigningCredentials(string secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        return new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
    }

    private string GenerateToken(
        SigningCredentials signingCredentials,
        IEnumerable<Claim> claims,
        DateTime expires
    )
    {
        var jwtHeader = new JwtHeader(signingCredentials);
        var jwtPayload = new JwtPayload(null, null, claims, DateTime.UtcNow, expires, DateTime.UtcNow);
        var jwtToken = new JwtSecurityToken(jwtHeader, jwtPayload);

        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return token;
    }
}
