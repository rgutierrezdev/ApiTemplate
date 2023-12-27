using ApiTemplate.Domain.Common.Events;

namespace ApiTemplate.Application.EventHandlers;

public class InvalidateUserEnabledCache :
    IEventNotificationHandler<EntityUpdatedEvent<User>>,
    IEventNotificationHandler<EntityDeletedEvent<User>>
{
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyService _cacheKeyService;

    public InvalidateUserEnabledCache(ICacheService cacheService, ICacheKeyService cacheKeyService)
    {
        _cacheService = cacheService;
        _cacheKeyService = cacheKeyService;
    }

    public Task Handle(
        EventNotification<EntityUpdatedEvent<User>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.Id);
    }

    public Task Handle(
        EventNotification<EntityDeletedEvent<User>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.Id);
    }

    private async Task InvalidateCache(Guid userId)
    {
        var cacheKey = _cacheKeyService.GetKey(CacheKeys.User, userId.ToString());
        await _cacheService.RemoveAsync(cacheKey);
    }
}
