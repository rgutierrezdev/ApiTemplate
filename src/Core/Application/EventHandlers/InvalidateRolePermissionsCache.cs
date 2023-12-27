using ApiTemplate.Domain.Common.Events;

namespace ApiTemplate.Application.EventHandlers;

public class InvalidateRolePermissionsCache :
    IEventNotificationHandler<EntityCreatedEvent<RolePermission>>,
    IEventNotificationHandler<EntityUpdatedEvent<RolePermission>>,
    IEventNotificationHandler<EntityDeletedEvent<RolePermission>>,
    IEventNotificationHandler<EntityDeletedEvent<Role>>
{
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyService _cacheKeyService;

    public InvalidateRolePermissionsCache(ICacheService cacheService, ICacheKeyService cacheKeyService)
    {
        _cacheService = cacheService;
        _cacheKeyService = cacheKeyService;
    }

    public Task Handle(
        EventNotification<EntityCreatedEvent<RolePermission>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.RoleId);
    }

    public Task Handle(
        EventNotification<EntityUpdatedEvent<RolePermission>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.RoleId);
    }

    public Task Handle(
        EventNotification<EntityDeletedEvent<RolePermission>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.RoleId);
    }

    public Task Handle(
        EventNotification<EntityDeletedEvent<Role>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.Id);
    }

    private async Task InvalidateCache(Guid roleId)
    {
        var cacheKey = _cacheKeyService.GetKey(CacheKeys.RolePermissions, roleId.ToString());
        await _cacheService.RemoveAsync(cacheKey);
    }
}
