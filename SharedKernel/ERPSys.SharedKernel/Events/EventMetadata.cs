// ============================================
// File: SharedKernel/Events/EventMetadata.cs
// Namespace: ERPSys.SharedKernel.Events
// ============================================

using System;

namespace ERPSys.SharedKernel.Events
{
    /// <summary>
    /// EN: Metadata attached to events for traceability and auditing.
    /// Includes EventId, EventType, OccurredOn, CorrelationId, UserId, Role, TenantId, SourceModule, Timestamp.
    ///
    /// </summary>
    public class EventMetadata
    {
        // Identity of the event
        public Guid EventId { get; init; } = Guid.NewGuid();
        public string EventType { get; init; } = string.Empty;
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

        // Traceability
        public Guid CorrelationId { get; init; } = Guid.NewGuid();
        public string? UserId { get; init; }
        public string? Role { get; init; }
        public string? TenantId { get; init; }
        public string SourceModule { get; init; } = string.Empty;

        // Technical timestamp
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
