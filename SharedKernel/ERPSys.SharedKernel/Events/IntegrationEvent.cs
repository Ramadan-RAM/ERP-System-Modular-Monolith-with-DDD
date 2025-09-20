// ============================================
// File: SharedKernel/Events/IntegrationEvent.cs
// Namespace: ERPSys.SharedKernel.Events
// Purpose: Base record for cross-module events
// ============================================

using System;

namespace ERPSys.SharedKernel.Events
{
    /// <summary>
    /// EN: Base for integration events published across modules (via RabbitMQ).
    /// Contains Id and OccurredOn.
    ///
    /// </summary>
    public abstract record IntegrationEvent(Guid Id, DateTime OccurredOn)
        : IDomainEvent
    {
        public IntegrationEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }
    }
}
