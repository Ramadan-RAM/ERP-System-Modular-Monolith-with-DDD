// ============================================
// File: SharedKernel/Messaging/IEventSubscriber.cs
// Namespace: ERPSys.SharedKernel.Messaging
// Purpose: Abstraction for subscribing/consuming events
// ============================================

using System.Threading;
using System.Threading.Tasks;
using ERPSys.SharedKernel.Events;

namespace ERPSys.SharedKernel.Messaging
{
    /// <summary>
    /// EN: Abstraction for subscribing to events (consumers).
    /// </summary>
    public interface IEventSubscriber
    {
        Task SubscribeAsync<TEvent>(string topic, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent;
    }
}
