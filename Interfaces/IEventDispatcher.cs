namespace KappaQueue.Events.Mediator.Interfaces
{
    public interface IEventDispatcher
    {
        void Invoke<Event>(Event @event) where Event : IEvent;
        Task InvokeAsync<Event>(Event @event, CancellationToken token = default) where Event : IEvent;
    }
}
