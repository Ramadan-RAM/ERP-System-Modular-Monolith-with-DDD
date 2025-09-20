using System.Collections.Concurrent;
using ERPSys.SharedKernel.Events;

namespace ERPSys.SharedKernel.Messaging
{
    /// <summary>
    /// EN: Simple in-memory event bus (no RabbitMQ, no Docker).
  
    /// </summary>
    public class InMemoryEventBus : IEventPublisher
    {
        private readonly ConcurrentDictionary<Type, List<Func<IDomainEvent, EventMetadata, Task>>> _handlers
            = new();

        /// <summary>
        /// Subscribe to an event type with a handler.
        /// </summary>
        public void Subscribe<TEvent>(Func<TEvent, EventMetadata, Task> handler)
            where TEvent : IDomainEvent
        {
            var handlers = _handlers.GetOrAdd(typeof(TEvent), _ => new List<Func<IDomainEvent, EventMetadata, Task>>());
            handlers.Add(async (evt, meta) => await handler((TEvent)evt, meta));
        }

        /// <summary>
        /// Publish event to all subscribed handlers.
        /// </summary>
        public async Task PublishAsync<TEvent>(TEvent @event, EventMetadata metadata, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent
        {
            if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                foreach (var handler in handlers)
                {
                    // ✅ Debug point Here you will see the events
                    Console.WriteLine($"[InMemoryBus] Dispatching {@event.GetType().Name} at {metadata.Timestamp}");
                    await handler(@event, metadata);
                }
            }
            else
            {
                Console.WriteLine($"⚠️ [InMemoryBus] No subscribers for {@event.GetType().Name}");
            }
        }
    }
}
