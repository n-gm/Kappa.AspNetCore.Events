using KappaQueue.Events.Mediator.Classes;
using KappaQueue.Events.Mediator.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace KappaQueue.Events.Mediator.AspNet
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddEventHandler(this IServiceCollection collection, EventHandlerConfig config)
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
