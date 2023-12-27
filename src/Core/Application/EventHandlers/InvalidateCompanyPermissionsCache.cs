using ApiTemplate.Domain.Common.Events;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Application.EventHandlers;

public class InvalidateCompanyPermissionsCache :
    IEventNotificationHandler<EntityCreatedEvent<CompanyUserPermission>>,
    IEventNotificationHandler<EntityUpdatedEvent<CompanyUserPermission>>,
    IEventNotificationHandler<EntityDeletedEvent<CompanyUserPermission>>,
    IEventNotificationHandler<EntityUpdatedEvent<CompanyUser>>
{
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyService _cacheKeyService;

    public InvalidateCompanyPermissionsCache(ICacheService cacheService, ICacheKeyService cacheKeyService)
    {
        _cacheService = cacheService;
        _cacheKeyService = cacheKeyService;
    }

    public Task Handle(
        EventNotification<EntityCreatedEvent<CompanyUserPermission>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.CompanyUserId);
    }

    public Task Handle(
        EventNotification<EntityUpdatedEvent<CompanyUserPermission>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.CompanyUserId);
    }

    public Task Handle(
        EventNotification<EntityDeletedEvent<CompanyUserPermission>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.CompanyUserId);
    }

    public Task Handle(
        EventNotification<EntityUpdatedEvent<CompanyUser>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.Id);
    }

    private async Task InvalidateCache(Guid companyUserId)
    {
        var cacheKey = _cacheKeyService.GetKey(CacheKeys.CompanyUserPermissions, companyUserId.ToString());
        await _cacheService.RemoveAsync(cacheKey);
    }
}
