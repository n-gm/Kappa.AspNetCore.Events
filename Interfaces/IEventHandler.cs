namespace KappaQueue.Events.Mediator.Interfaces
{
    public interface IEventHandler
    {
    }

    public interface IAsyncEventHandler : IEventHandler
    {

    }

    public interface IEventHandler<Event> : IEventHandler
        where Event : IEvent
    {
        void Handle(Event @event);
    }

    public interface IAsyncEventHandler<Event> : IAsyncEventHandler
        where Event : IEvent
    {
        Task HandleAsync(Event @event, CancellationToken token = default);
    }
}
