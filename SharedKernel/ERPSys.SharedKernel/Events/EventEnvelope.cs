// ============================================
// File: SharedKernel/Events/EventEnvelope.cs
// Namespace: ERPSys.SharedKernel.Events
// Purpose: Wrapper to carry event data + metadata
// ============================================

using System;
using System.Diagnostics.Eventing.Reader;

namespace ERPSys.SharedKernel.Events
{
    /// <summary>
    /// EN: Envelope that wraps an event together with its metadata
    /// (correlation, user, module info).
    /// 
    /// </summary>
    public class EventEnvelope<TEvent> where TEvent : IDomainEvent
    {
        public TEvent Event { get; }
        public EventMetadata Metadata { get; }

        public EventEnvelope(TEvent @event, EventMetadata metadata)
        {
            Event = @event ?? throw new ArgumentNullException(nameof(@event));
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }
    }
}
