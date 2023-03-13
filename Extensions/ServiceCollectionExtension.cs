using Microsoft.Extensions.DependencyInjection;

namespace Kappa.AspNetCore.Events.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddEventHandlers(this IServiceCollection collection, Action<EventHandlerConfig> action)
        {
            EventHandlerConfig config = new();
            action?.Invoke(config);

            collection.AddSingleton(config);
            foreach (var type in config.GetEventTypes())
            {
                foreach (var handlerType in config.GetHandlerTypes(type))
                {
                    collection.AddScoped(handlerType);
                }
            }
            collection.AddScoped<IEventDispatcher, EventDispatcher>();

            return collection;
        }

        public static IServiceCollection AddEventHandlers(this IServiceCollection collection, EventHandlerConfig config)
        {
            collection.AddSingleton(config);
            foreach(var type in config.GetEventTypes())
            {
                foreach (var handlerType in config.GetHandlerTypes(type))
                {
                    collection.AddScoped(handlerType);
                }
            }
            collection.AddScoped<IEventDispatcher, EventDispatcher>();

            return collection;
        }
    }
}
