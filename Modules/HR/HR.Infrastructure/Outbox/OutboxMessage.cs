using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Infrastructure.Outbox
{
    public class OutboxMessage
    {
        public long Id { get; set; }
        public Guid EventId { get; set; }
        public string Type { get; set; } = default!;
        public string Payload { get; set; } = default!;
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
        public bool Sent { get; set; } = false;
        public DateTime? SentOn { get; set; }
    }
}

