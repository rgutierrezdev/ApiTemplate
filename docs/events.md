# Events

## Default Domain Events

As part of DDD there is the concept of domain Domain Events where you can define events attached to a domain and then
listen to those whenever they are triggered.

By default, when you create, update or delete an entity, a domain event will be triggered, you just need to subscribe to
those events according to your requirements.

For instance, whenever a new user is created you want to send a welcome email. You can simply add the Send Welcome Email
logic when the user is created, but this means you need to add/call that logic everytime a user is created and you need
to know where in the whole project an User is being created. So it's better to simply subscribe to the "Created User
Event" and you don't need to worry about all the places an User is created.

You can do that by using the `IEventNotificationHandler` interface and `EntityCreatedEvent`, `EntityUpdatedEvent` or `
EntityDeletedEvent` and pass the `IEntity` Class as a generic depending on the scenario. For this case we can use
the `EntityCreatedEvent<User>`:

```csharp
public class SendWelcomeEmail : IEventNotificationHandler<EntityCreatedEvent<User>>
{
    private readonly IMailService _mailService;
    
    public SendWelcomeEmail(IMailService mailService)
    {
        _mailService = mailService;
    }
    
    public Task Handle(
        EventNotification<EntityCreatedEvent<User>> notification,
        CancellationToken cancellationToken
    )
    {
        // add your mail creation logic        
        ...
            
        // get the entity data from notificacion.Event.Entity
        // notification.Event.Entity.Id
        // notification.Event.Entity.Email
        
        await _mailService.SendAsync(...);
    }
}
```

## Custom Domain Events

You can also define your custom events by using the `DomainEvent` class:

```csharp
public class TicketClosedEvent : DomainEvent
{
    public Guid TicketId { get; set; } = default!;
    public DateTime ClosedDate { get; set; } = default!;
    
    public ApplicationUserCreatedEvent(string userId, DateTime closedDate)
    {
        TicketId = userId;
        ClosedDate = closedDate;
    }
}
```

Trigger the event whenever it is necessary using the `IEventPublisher` service:

```csharp
await _eventPublisher.PublishAsync(new TicketClosedEvent(ticketId, closedDate));
```

An then listen for it:

```csharp
public class SendTicketClosedEmail : IEventNotificationHandler<TicketClosedEvent>
{
    private readonly IMailService _mailService;
    
    public SendTicketClosedEmail(IMailService mailService)
    {
        _mailService = mailService;
    }
    
    public Task Handle(
        EventNotification<TicketClosedEvent> notification,
        CancellationToken cancellationToken
    )
    {
        // add your mail creation logic
        ...
        
        // get the data from notification.Event
        // notification.Event.TicketId
        // notification.Event.ClosedDate
        
        await _mailService.SendAsync(...);
    }
}
```

___

If you need to execute the same logic for multiple events, simply implement the `IEventNotificationHandler` into the
same class for all the events you need:

```csharp
public class InvalidateDocumentTypesCache :
    IEventNotificationHandler<EntityCreatedEvent<DocumentType>>,
    IEventNotificationHandler<EntityUpdatedEvent<DocumentType>>,
    IEventNotificationHandler<EntityDeletedEvent<DocumentType>>
{

    public Task Handle(
        EventNotification<EntityCreatedEvent<DocumentType>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity);
    }

    public Task Handle(
        EventNotification<EntityUpdatedEvent<DocumentType>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity);
    }

    public Task Handle(
        EventNotification<EntityDeletedEvent<DocumentType>> notification,
        CancellationToken cancellationToken
    )
    {
        return InvalidateCache(notification.Event.Entity);
    }

    private async Task InvalidateCache(DocumentType documentType)
    {
        // add your invalidation logic
    }
}
```


