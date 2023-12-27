using System.Security.Claims;
using ApiTemplate.Application.Features.Auth.Dtos;

namespace ApiTemplate.Application.Features.Auth;

public class RefreshTokens
{
    public class Request : IRequest<TokensExpirationDto>
    {
    }

    internal class Handler : IRequestHandler<Request, TokensExpirationDto>
    {
        private readonly IRepository<BlacklistedToken> _blacklistedTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICookieService _cookieService;

        public Handler(
            IRepository<BlacklistedToken> blacklistedTokenRepository,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            ICookieService cookieService
        )
        {
            _blacklistedTokenRepository = blacklistedTokenRepository;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _cookieService = cookieService;
        }

        public async Task<TokensExpirationDto> Handle(Request request, CancellationToken cancellationToken)
        {
            var refreshToken = _cookieService.GetRefreshToken();
            var claims = _tokenService.VerifyRefreshToken(refreshToken).ToList();

            var sId = Guid.Parse(claims.First(c => c.Type == ClaimTypes.Sid).Value);
            var currentBlacklistedToken = await _blacklistedTokenRepository.GetByIdAsync(sId, cancellationToken);

            // In case multiple refresh token requests come at the same time with the same refresh token and current token
            // is blacklisted, we give it a 15s tolerance
            if (currentBlacklistedToken != null)
            {
                var currentDate = DateTime.UtcNow.AddSeconds(-15);

                if (currentBlacklistedToken.CreatedDate < currentDate)
                {
                    throw new UnauthorizedException(ErrorCodes.InvalidRefreshToken);
                }
            }

            var userId = Guid.Parse(claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var expiryTicks = long.Parse(claims.First(c => c.Type == "exp").Value);
            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryTicks).UtcDateTime;

            var blacklistedToken = new BlacklistedToken()
            {
                Id = sId,
                UserId = userId,
                ExpiryDate = expiryDate,
                CreatedDate = DateTime.UtcNow,
            };

            _blacklistedTokenRepository.Add(blacklistedToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var tokenResponse = _tokenService.GenerateAuthTokens(userId);

            _cookieService.AttachTokenCookies(tokenResponse.AccessToken, tokenResponse.RefreshToken);

            var tokensExpiration = tokenResponse.Adapt<TokensExpirationDto>();

            return tokensExpiration;
        }
    }
}
