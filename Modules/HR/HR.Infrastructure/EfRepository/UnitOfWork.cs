using ERPSys.SharedKernel.Persistence;
using HR.Infrastructure.HR.DbContext;


namespace HR.Infrastructure.EfRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HRDbContext _context;

        public UnitOfWork(HRDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
