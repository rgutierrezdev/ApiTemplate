using ApiTemplate.Domain.Common.Events;

namespace ApiTemplate.Application.EventHandlers;

public class InvalidateUserRoleIdsCache :
    IEventNotificationHandler<EntityCreatedEvent<UserRole>>,
    IEventNotificationHandler<EntityUpdatedEvent<UserRole>>,
    IEventNotificationHandler<EntityDeletedEvent<UserRole>>,
    IEventNotificationHandler<EntityDeletedEvent<User>>
{
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyService _cacheKeyService;

    public InvalidateUserRoleIdsCache(ICacheService cacheService, ICacheKeyService cacheKeyService)
    {
        _cacheService = cacheService;
        _cacheKeyService = cacheKeyService;
    }

    public Task Handle(
        EventNotification<EntityCreatedEvent<UserRole>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.UserId);
    }

    public Task Handle(
        EventNotification<EntityUpdatedEvent<UserRole>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.UserId);
    }

    public Task Handle(
        EventNotification<EntityDeletedEvent<UserRole>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.UserId);
    }

    public Task Handle(EventNotification<EntityDeletedEvent<User>> notification, CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Event.Entity.Id);
    }

    private async Task InvalidateCache(Guid userId)
    {
        var cacheKey = _cacheKeyService.GetKey(CacheKeys.UserRoleIds, userId.ToString());
        await _cacheService.RemoveAsync(cacheKey);
    }
}
