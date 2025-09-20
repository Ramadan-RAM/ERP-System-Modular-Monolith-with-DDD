// ============================================
// File: SharedKernel/Events/DomainEvent.cs
// Namespace: ERPSys.SharedKernel.Events
// Purpose: Base class implementation for domain events
// ============================================

using System;

namespace ERPSys.SharedKernel.Events
{
    /// <summary>
    /// EN: Base implementation of IDomainEvent.
    /// Provides OccurredOn timestamp when the event was created.
    ///
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
    }
}
