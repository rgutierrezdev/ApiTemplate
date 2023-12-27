using System.Security.Claims;

namespace ApiTemplate.Application.Features.Auth;

public class Logout
{
    public class Request : IRequest
    {
    }

    internal class Handler : IRequestHandler<Request>
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

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var refreshToken = _cookieService.GetRefreshToken();
            var claims = _tokenService.VerifyRefreshToken(refreshToken).ToList();

            var sId = Guid.Parse(claims.First(c => c.Type == ClaimTypes.Sid).Value);
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

            _cookieService.ClearTokenCookies();

            return Unit.Value;
        }
    }
}
