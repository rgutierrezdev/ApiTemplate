using System.Security.Claims;
using Ardalis.Specification;
using Microsoft.AspNetCore.Http;
using ApiTemplate.Application.Common.Cache;
using ApiTemplate.Application.Common.Constants;
using ApiTemplate.Application.Common.Interfaces;
using ApiTemplate.Application.Common.Persistence;
using ApiTemplate.Domain.Common.Exceptions;
using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Infrastructure.Auth.CurrentUser;

public class CurrentUserMiddleware : IMiddleware
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<User> _userRepository;
    private readonly ICacheKeyService _cacheKeyService;
    private readonly ICacheService _cacheService;

    public CurrentUserMiddleware(
        ICurrentUser currentUser,
        IRepository<User> userRepository,
        ICacheKeyService cacheKeyService,
        ICacheService cacheService
    )
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
        _cacheKeyService = cacheKeyService;
        _cacheService = cacheService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers.UserAgent.ToString();

        _currentUser.SetClient(clientIp, userAgent);

        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var subject = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new UnauthorizedException(ErrorCodes.AuthenticationFailed);
            }

            var userId = Guid.Parse(subject);
            var user = await GetUserAsync(userId);

            if (user == null || user.Enabled == false)
            {
                throw new UnauthorizedException(ErrorCodes.AuthenticationFailed);
            }

            await _currentUser.SetCurrentAsync(userId, user.Email, user.FirstName, user.LastName);
        }

        await next(context);
    }

    private record UserData(string Email, string FirstName, string LastName, bool Enabled);

    private async Task<UserData?> GetUserAsync(Guid userId)
    {
        var cacheKey = _cacheKeyService.GetKey(CacheKeys.User, userId.ToString());
        var user = await _cacheService.GetAsync<UserData?>(cacheKey);

        if (user != null)
            return user;

        user = await _userRepository.FirstOrDefaultAsync<UserData>(q => q.Where(u => u.Id == userId));

        await _cacheService.SetAsync(cacheKey, user);

        return user;
    }
}
