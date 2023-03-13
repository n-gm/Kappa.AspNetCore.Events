using Kappa.AspNetCore.Events.Classes;

namespace Kappa.AspNetCore.Events
{

    /// <summary>
    /// Класс с конфигурацией событий приложения
    /// </summary>
    public class EventHandlerConfig
    {
        private Dictionary<Type, List<Type>> _handlers;

        public EventHandlerConfig()
        {
            _handlers = new();
        }

        internal IEnumerable<Type> GetEventTypes()
        {
            return _handlers.Keys;
        }

        internal IEnumerable<Type> GetHandlerTypes(Type eventType)
        {
            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                return handlers;
            }
            else
            {
                return Enumerable.Empty<Type>();
            }
        }

        internal IEnumerable<Type> GetHandlerTypes<EventType>() 
            where EventType : IEvent
        {
            return GetHandlerTypes(typeof(EventType));
        }

        public EventHandlerConfig AddAsyncEventHandler<AsyncEventHandlerType, EventType>()
            where AsyncEventHandlerType : IAsyncEventHandler<EventType>
            where EventType : IEvent
        {
            if (_handlers.TryGetValue(typeof(EventType), out var handlers))
            {
                handlers.Add(typeof(AsyncEventHandlerType));
            }
            else
            {
                _handlers.Add(typeof(EventType), new List<Type>() { typeof(AsyncEventHandlerType) });
            }

            return this;
        }

        public EventHandlerConfig AddAsyncEventHandler(Type asyncEvent, Type asyncEventHandler)
        {
            if (_handlers.TryGetValue(asyncEvent, out var handlers))
            {
                handlers.Add(asyncEventHandler);
            }
            else
            {
                _handlers.Add(asyncEvent, new List<Type>() { asyncEventHandler });
            }

            return this;
        }

        public EventHandlerConfig AddEventHandler<EventHandlerType, EventType>()
            where EventHandlerType : IEventHandler<EventType>
            where EventType : IEvent
        {
            if (_handlers.TryGetValue(typeof(EventType), out var handlers))
            {
                handlers.Add(typeof(EventHandlerType));
            }
            else
            {
                _handlers.Add(typeof(EventType), new List<Type>() { typeof(EventHandlerType) });
            }

            return this;
        }
    }
}
