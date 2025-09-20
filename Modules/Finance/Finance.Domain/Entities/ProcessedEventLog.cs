using System;

namespace Finance.Domain.Entities
{
    public class ProcessedEventLog
    {
        public Guid EventId { get; private set; }
        public string HandlerName { get; private set; } = string.Empty;
        public DateTime ProcessedOn { get; private set; } = DateTime.UtcNow;

        private ProcessedEventLog() { } // EF Core

        public ProcessedEventLog(Guid eventId, string handlerName)
        {
            EventId = eventId;
            HandlerName = handlerName;
            ProcessedOn = DateTime.UtcNow;
        }
    }
}
