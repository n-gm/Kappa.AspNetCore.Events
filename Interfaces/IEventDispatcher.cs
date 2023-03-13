namespace Kappa.AspNetCore.Events
{
    public interface IEventDispatcher
    {
        void Dispatch<Event>(Event @event) where Event : IEvent;
        Task DispatchAsync<Event>(Event @event, CancellationToken token = default) where Event : IEvent;
        Task DispatchInNewScopeAsync<Event>(Event @event) where Event : IEvent;
    }
}
