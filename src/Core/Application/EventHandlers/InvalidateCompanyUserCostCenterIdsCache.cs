using ApiTemplate.Domain.Common.Events;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Application.EventHandlers;

public class InvalidateCompanyUserCostCenterIdsCache :
    IEventNotificationHandler<EntityCreatedEvent<CostCenterUser>>,
    IEventNotificationHandler<EntityUpdatedEvent<CostCenterUser>>,
    IEventNotificationHandler<EntityDeletedEvent<CostCenterUser>>
{
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyService _cacheKeyService;

    public InvalidateCompanyUserCostCenterIdsCache(ICacheService cacheService, ICacheKeyService cacheKeyService)
    {
        _cacheService = cacheService;
        _cacheKeyService = cacheKeyService;
    }

    public Task Handle(
        EventNotification<EntityCreatedEvent<CostCenterUser>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.CompanyUserId);
    }

    public Task Handle(
        EventNotification<EntityUpdatedEvent<CostCenterUser>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.CompanyUserId);
    }

    public Task Handle(
        EventNotification<EntityDeletedEvent<CostCenterUser>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.CompanyUserId);
    }

    private async Task InvalidateCache(Guid companyUserId)
    {
        var cacheKey = _cacheKeyService.GetKey(CacheKeys.CompanyUserCostCenterIds, companyUserId.ToString());
        await _cacheService.RemoveAsync(cacheKey);
    }
}
