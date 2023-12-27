namespace ApiTemplate.Application.Common.Events;

public class EventNotification<TEvent> : INotification
    where TEvent : IEvent
{
    public TEvent Event { get; }

    public EventNotification(TEvent @event)
    {
        Event = @event;
    }
}
