namespace Kappa.AspNetCore.Events
{
    public interface IEventHandler
    {
        Type EventType { get; }
    }

    public interface IAsyncEventHandler : IEventHandler
    {
    }

    public interface IEventHandler<Event> : IEventHandler
        where Event : IEvent
    {
        new Type EventType => typeof(Event);
        void Handle(Event @event);
    }

    public interface IAsyncEventHandler<Event> : IAsyncEventHandler
        where Event : IEvent
    {
        new Type EventType => typeof(Event);
        Task HandleAsync(Event @event, CancellationToken token = default);
    }
}
