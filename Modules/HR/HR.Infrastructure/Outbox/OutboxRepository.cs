using HR.Infrastructure.HR.DbContext;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Outbox
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly HRDbContext _context;

        public OutboxRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OutboxMessage msg)
        {
            await _context.Set<OutboxMessage>().AddAsync(msg);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OutboxMessage>> GetUnsentAsync(int max = 50)
        {
            return await _context.Set<OutboxMessage>()
                .Where(m => !m.Sent)
                .OrderBy(m => m.OccurredOn)
                .Take(max)
                .ToListAsync();
        }

        public async Task MarkSentAsync(long id)
        {
            var msg = await _context.Set<OutboxMessage>().FindAsync(id);
            if (msg != null)
            {
                msg.Sent = true;
                msg.SentOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
