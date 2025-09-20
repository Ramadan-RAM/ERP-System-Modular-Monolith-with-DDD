using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ERPSys.SharedKernel.Events;
using Microsoft.Extensions.Logging;

namespace ERPSys.SharedKernel.Messaging
{
    public class InMemoryEventPublisher : IEventPublisher
    {
        private readonly ILogger<InMemoryEventPublisher>? _logger;
        private readonly ConcurrentDictionary<Type, List<Func<IDomainEvent, EventMetadata, Task>>> _handlers
            = new();

        public InMemoryEventPublisher(ILogger<InMemoryEventPublisher>? logger = null) => _logger = logger;

        public void Subscribe<TEvent>(Func<TEvent, EventMetadata, Task> handler) where TEvent : IDomainEvent
        {
            var list = _handlers.GetOrAdd(typeof(TEvent), _ => new List<Func<IDomainEvent, EventMetadata, Task>>());
            list.Add(async (evt, meta) => await handler((TEvent)evt, meta));
            _logger?.LogInformation("[InMemory] Subscribed {Event}", typeof(TEvent).Name);
        }

        public async Task PublishAsync<TEvent>(TEvent @event, EventMetadata metadata, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent
        {
            var type = typeof(TEvent);
            _logger?.LogInformation("[InMemory] Publish {Event}", type.Name);
            if (_handlers.TryGetValue(type, out var handlers))
            {
                foreach (var h in handlers)
                    await h(@event, metadata);
            }
            else
            {
                _logger?.LogWarning("[InMemory] No subscribers for {Event}", type.Name);
            }
        }
    }
}
