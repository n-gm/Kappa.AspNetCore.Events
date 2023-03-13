using KappaQueue.Events.Mediator.Interfaces;

namespace KappaQueue.Events.Mediator.Classes
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

        public IEnumerable<Type> GetEventTypes()
        {
            return _handlers.Keys;
        }

        public IEnumerable<Type> GetHandlerTypes(Type eventType)
        {
            if (_handlers.TryGetValue(eventType, out var listeners))
            {
                return listeners;
            }
            else
            {
                return Enumerable.Empty<Type>();
            }
        }

        public IEnumerable<Type> GetHandlerTypes<EventType>() where EventType : IEvent
        {
            return GetHandlerTypes(typeof(EventType));
        }

        public EventHandlerConfig AddAsyncEventHandler<AsyncEventListenerType, EventType>()
            where AsyncEventListenerType : IAsyncEventHandler<EventType>
            where EventType : IEvent
        {
            if (_handlers.TryGetValue(typeof(EventType), out var listeners))
            {
                listeners.Add(typeof(AsyncEventListenerType));
            }
            else
            {
                _handlers.Add(typeof(EventType), new List<Type>() { typeof(AsyncEventListenerType) });
            }

            return this;
        }

        public EventHandlerConfig AddEventHandler<EventListenerType, EventType>() 
            where EventListenerType : IEventHandler<EventType>
            where EventType : IEvent
        {
            if (_handlers.TryGetValue(typeof(EventType), out var listeners))
            {
                listeners.Add(typeof(EventListenerType));
            }
            else
            {
                _handlers.Add(typeof(EventType), new List<Type>() { typeof(EventListenerType) });
            }

            return this;
        }

        public EventHandlerConfig AddEventHandler<EventType>(IEventHandler<EventType> eventListener) where EventType : IEvent
        {
            return AddEventHandler<IEventHandler<EventType>, EventType>();
        }

        public EventHandlerConfig AddEventHandler<EventType>(IAsyncEventHandler<EventType> eventListener) where EventType : IEvent
        {
            return AddAsyncEventHandler<IAsyncEventHandler<EventType>, EventType>();
        }        
    }
}
