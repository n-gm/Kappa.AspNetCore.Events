using Microsoft.Extensions.DependencyInjection;

namespace Kappa.AspNetCore.Events
{
    internal class EventDispatcher : IEventDispatcher
    {
        private readonly EventHandlerConfig? _config;
        private readonly IServiceProvider _provider;

        public EventDispatcher(IServiceProvider provider)
        {
            _provider = provider;
        }

        public EventDispatcher(EventHandlerConfig config,
                               IServiceProvider provider)
        {
            _config = config;
            _provider = provider;
        }

        /// <summary>
        /// Dispatch sync event. Event will be processed as sync event if event is IAsyncEventHandler.
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        /// <param name="event"></param>
        public void Dispatch<Event>(Event @event) 
            where Event : IEvent
        {
            if (_config != null)
            {
                InvokeByConfig(@event, _config);
            }
            else
            {
                InvokeWithoutConfig(@event);
            }
        }

        /// <summary>
        /// Dispatch async event.
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        /// <param name="event"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task DispatchAsync<Event>(Event @event, CancellationToken token = default) 
            where Event : IEvent
        {
            if (_config != null)
            {
                await InvokeByConfigAsync(@event, _config, _provider, token);
            }
            else
            {
                await InvokeWithoutConfigAsync(@event, _provider, token);
            }
        }

        /// <summary>
        /// Dispatch event in new ServiceScope. Made for Fire-And-Forget strategy.
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task DispatchInNewScopeAsync<Event>(Event @event)
            where Event : IEvent
        {
            await Task.Run(async () =>
            {
                using var scope = _provider.CreateScope();

                if (_config != null)
                {
                    await InvokeByConfigAsync(@event, _config, scope.ServiceProvider);
                }
                else
                {
                    await InvokeWithoutConfigAsync(@event, scope.ServiceProvider);
                }
            }
            );
        }

        private async Task InvokeByConfigAsync<Event>(Event @event,
                                                      EventHandlerConfig config,
                                                      IServiceProvider provider,
                                                      CancellationToken token = default)
            where Event : IEvent
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            foreach (var handlerType in config.GetHandlerTypes<Event>())
            {
                var handler = provider.GetRequiredService(handlerType);

                if (handler is IAsyncEventHandler<Event> asyncHandler)
                {
                    await asyncHandler.HandleAsync(@event, token);
                }
                else if (handler is IEventHandler<Event> syncHandler)
                {
                    syncHandler.Handle(@event);
                }
            }
        }

        private async Task InvokeWithoutConfigAsync<Event>(Event @event,
                                                           IServiceProvider provider,
                                                           CancellationToken token = default)
            where Event : IEvent
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            foreach (var handler in provider.GetServices<IEventHandler<Event>>())
            {
                handler.Handle(@event);
            }

            foreach (var handler in provider.GetServices<IAsyncEventHandler<Event>>())
            {
                await handler.HandleAsync(@event, token);
            }
        }

        private void InvokeByConfig<Event>(Event @event,
                                           EventHandlerConfig config) 
            where Event : IEvent
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            foreach (var handlerType in config.GetHandlerTypes<Event>())
            {
                var handler = _provider.GetRequiredService(handlerType);
                if (handler is IAsyncEventHandler<Event> asyncHandler)
                {
                    asyncHandler.HandleAsync(@event).Wait();
                }
                else if (handler is IEventHandler<Event> syncHandler)
                {
                    syncHandler.Handle(@event);
                }
            }
        }

        private void InvokeWithoutConfig<Event>(Event @event)
            where Event : IEvent
        {
            foreach (var handler in _provider.GetServices<IEventHandler<Event>>())
            {
                handler.Handle(@event);                
            }

            foreach(var handler in _provider.GetServices<IAsyncEventHandler<Event>>())
            {
                handler.HandleAsync(@event).Wait();
            }
        }
    }
}
