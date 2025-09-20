// ============================================
// File: SharedKernel/Events/IDomainEvent.cs
// Namespace: ERPSys.SharedKernel.Events
// Purpose: Contract for all domain events
// ============================================

using System;

namespace ERPSys.SharedKernel.Events
{
    /// <summary>
    /// EN: Contract for domain events raised inside aggregates.
    /// Every domain event should have an OccurredOn timestamp.
    ///
    /// </summary>
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
