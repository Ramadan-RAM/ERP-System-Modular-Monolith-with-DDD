using Finance.Application.ProcessedEvents;
using Finance.Domain.Entities;
using Finance.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Finance.Infrastructure.Repositories
{
    public class ProcessedEventRepository : IProcessedEventRepository
    {
        private readonly FinanceDbContext _db;

        public ProcessedEventRepository(FinanceDbContext db)
        {
            _db = db;
        }

        public async Task<bool> ExistsAsync(Guid eventId)
        {
            return await _db.Set<ProcessedEventLog>().AnyAsync(x => x.EventId == eventId);
        }

        public async Task MarkProcessedAsync(Guid eventId, string handlerName)
        {
            var log = new ProcessedEventLog(eventId, handlerName);
            _db.Set<ProcessedEventLog>().Add(log);
            await _db.SaveChangesAsync();
        }
    }
}
