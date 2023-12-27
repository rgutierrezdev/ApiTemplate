using ApiTemplate.Domain.Common.Events;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Application.EventHandlers;

public class InvalidateCompanyCache :
    IEventNotificationHandler<EntityCreatedEvent<Company>>,
    IEventNotificationHandler<EntityUpdatedEvent<Company>>,
    IEventNotificationHandler<EntityDeletedEvent<Company>>
{
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyService _cacheKeyService;

    public InvalidateCompanyCache(ICacheService cacheService, ICacheKeyService cacheKeyService)
    {
        _cacheService = cacheService;
        _cacheKeyService = cacheKeyService;
    }

    public Task Handle(
        EventNotification<EntityCreatedEvent<Company>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.Id);
    }

    public Task Handle(
        EventNotification<EntityUpdatedEvent<Company>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.Id);
    }

    public Task Handle(
        EventNotification<EntityDeletedEvent<Company>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity.Id);
    }

    private async Task InvalidateCache(Guid companyId)
    {
        var cacheKey = _cacheKeyService.GetKey(CacheKeys.Company, companyId.ToString());
        await _cacheService.RemoveAsync(cacheKey);
    }
}
