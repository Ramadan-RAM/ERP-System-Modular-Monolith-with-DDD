// ============================================
// File: SharedKernel/Messaging/IEventPublisher.cs
// Namespace: ERPSys.SharedKernel.Messaging
// Purpose: Abstraction for publishing domain/integration events
// ============================================

using System.Threading;
using System.Threading.Tasks;
using ERPSys.SharedKernel.Events;

namespace ERPSys.SharedKernel.Messaging
{
    /// <summary>
    /// EN: Abstraction for publishing events to a message bus (e.g., RabbitMQ).
    /// </summary>
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event, EventMetadata metadata, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent;
    }
}
