using System;

namespace Finance.Application.ProcessedEvents
{
    public class ProcessedEvent
    {
        public Guid EventId { get; set; }
        public DateTime ProcessedOn { get; set; } = DateTime.UtcNow;
        public string Handler { get; set; } = default!;
    }
}
