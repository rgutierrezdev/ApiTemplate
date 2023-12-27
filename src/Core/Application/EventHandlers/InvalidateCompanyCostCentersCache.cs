using ApiTemplate.Domain.Common.Events;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Application.EventHandlers;

public class InvalidateCompanyCostCentersCache :
    IEventNotificationHandler<EntityCreatedEvent<CostCenter>>,
    IEventNotificationHandler<EntityUpdatedEvent<CostCenter>>,
    IEventNotificationHandler<EntityDeletedEvent<CostCenter>>
{
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyService _cacheKeyService;

    public InvalidateCompanyCostCentersCache(ICacheService cacheService, ICacheKeyService cacheKeyService)
    {
        _cacheService = cacheService;
        _cacheKeyService = cacheKeyService;
    }

    public Task Handle(
        EventNotification<EntityCreatedEvent<CostCenter>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.CompanyId);
    }

    public Task Handle(
        EventNotification<EntityUpdatedEvent<CostCenter>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.CompanyId);
    }

    public Task Handle(
        EventNotification<EntityDeletedEvent<CostCenter>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.CompanyId);
    }

    private async Task InvalidateCache(Guid companyId)
    {
        var cacheKey = _cacheKeyService.GetKey(CacheKeys.CompanyCostCenters, companyId.ToString());
        await _cacheService.RemoveAsync(cacheKey);
    }
}
