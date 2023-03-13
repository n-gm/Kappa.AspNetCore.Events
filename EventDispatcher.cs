using KappaQueue.Events.Mediator.Classes;
using KappaQueue.Events.Mediator.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace KappaQueue.Events.Mediator
{
    internal class EventDispatcher : IEventDispatcher
    {
        private readonly EventHandlerConfig _config;
        private readonly IServiceProvider _provider;

        public EventDispatcher(EventHandlerConfig config,
                               IServiceProvider provider)
        {
            _config = config;
            _provider = provider;
        }

        public void Invoke<Event>(Event @event) where Event : IEvent
        {
            foreach (var handlerType in _config.GetHandlerTypes<Event>())
            {
                var handler = _provider.GetRequiredService(handlerType);
                if (handler is IAsyncEventHandler)
                {                     
                    ((IAsyncEventHandler<Event>)handler).HandleAsync(@event).Wait();
                }
                else
                {
                    ((IEventHandler<Event>)handler).Handle(@event);
                }
            }            
        }

        public async Task InvokeAsync<Event>(Event @event, CancellationToken token = default) where Event : IEvent
        {
            foreach (var handlerType in _config.GetHandlerTypes<Event>())
            {
                var handler = _provider.GetRequiredService(handlerType);
                if (handler is IAsyncEventHandler)
                {
                    await ((IAsyncEventHandler<Event>)handler).HandleAsync(@event, token);
                }
                else
                {
                    ((IEventHandler<Event>)handler).Handle(@event);
                }
            }
        }
    }
}
