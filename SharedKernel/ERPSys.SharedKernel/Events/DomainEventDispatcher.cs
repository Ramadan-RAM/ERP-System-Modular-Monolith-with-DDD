using ERPSys.SharedKernel.Domain;
using ERPSys.SharedKernel.Messaging;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ERPSys.SharedKernel.Events
{
    public class DomainEventDispatcher
    {
        private readonly IEventPublisher _publisher;

        public DomainEventDispatcher(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        // Accepts an explicit set of domain events
        public async Task DispatchAsync(IEnumerable<IDomainEvent> events, EventMetadata metadata)
        {
            foreach (var domainEvent in events)
            {
                await _publisher.PublishAsync(domainEvent, metadata);
            }
        }

        // Convenience: dispatch from a BaseEntity that exposes DomainEvents
        public async Task DispatchAsync<TId>(BaseEntity<TId> entity, EventMetadata metadata)
        {
            if (entity == null) return;
            await DispatchAsync(entity.DomainEvents, metadata);
            entity.ClearDomainEvents();
        }

        // And for collections of entities
        public async Task DispatchAsync<TId>(IEnumerable<BaseEntity<TId>> entities, EventMetadata metadata)
        {
            foreach (var e in entities)
            {
                await DispatchAsync(e, metadata);
            }
        }
    }
}
