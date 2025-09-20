// ============================================
// File: SharedKernel/Messaging/IMessageBus.cs
// Namespace: ERPSys.SharedKernel.Messaging
// Purpose: High-level abstraction for message bus
// ============================================

using System.Threading;
using System.Threading.Tasks;
using ERPSys.SharedKernel.Events;

namespace ERPSys.SharedKernel.Messaging
{
    /// <summary>
    /// EN: High-level abstraction for message bus (publish + subscribe).
    /// </summary>
    public interface IMessageBus : IEventPublisher, IEventSubscriber
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
