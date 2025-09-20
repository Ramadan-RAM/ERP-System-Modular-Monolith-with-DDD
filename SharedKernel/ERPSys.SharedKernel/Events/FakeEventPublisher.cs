using ERPSys.SharedKernel.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSys.SharedKernel.Events
{
    public class FakeEventPublisher : IEventPublisher
    {
        public Task PublishAsync<TEvent>(TEvent @event, EventMetadata metadata, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent
        {
            // For testing: just log or store events instead of real publishing
            Console.WriteLine($"[FakeEventPublisher] Published {@event.GetType().Name} at {metadata.Timestamp}");
            return Task.CompletedTask;
        }

       
    }

}
