using ApiTemplate.Domain.Common.Events;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Application.EventHandlers;

public class InvalidateUserCompaniesCache :
    IEventNotificationHandler<EntityCreatedEvent<CompanyUser>>,
    IEventNotificationHandler<EntityUpdatedEvent<CompanyUser>>,
    IEventNotificationHandler<EntityDeletedEvent<CompanyUser>>
{
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyService _cacheKeyService;

    public InvalidateUserCompaniesCache(ICacheService cacheService, ICacheKeyService cacheKeyService)
    {
        _cacheService = cacheService;
        _cacheKeyService = cacheKeyService;
    }

    public Task Handle(
        EventNotification<EntityCreatedEvent<CompanyUser>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.UserId);
    }

    public Task Handle(
        EventNotification<EntityUpdatedEvent<CompanyUser>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.UserId);
    }

    public Task Handle(
        EventNotification<EntityDeletedEvent<CompanyUser>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.UserId);
    }

    private async Task InvalidateCache(Guid userId)
    {
        var cacheKey = _cacheKeyService.GetKey(CacheKeys.UserCompanies, userId.ToString());
        await _cacheService.RemoveAsync(cacheKey);
    }
}
