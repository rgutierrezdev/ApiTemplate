using System.Security.Claims;

namespace ApiTemplate.Application.Common.Interfaces;

public interface ITokenService : IScopedService
{
    TokenResponse GenerateAuthTokens(Guid userId);

    public IEnumerable<Claim> VerifyRefreshToken(string token);
}
