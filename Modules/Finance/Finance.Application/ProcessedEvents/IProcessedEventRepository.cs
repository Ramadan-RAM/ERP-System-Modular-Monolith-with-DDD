using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.ProcessedEvents
{
    public interface IProcessedEventRepository
    {
        Task<bool> ExistsAsync(Guid eventId);
        Task MarkProcessedAsync(Guid eventId, string handlerName);
    }
}
