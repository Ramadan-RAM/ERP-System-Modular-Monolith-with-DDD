// في Finance.Infrastructure.Extensions
using ERPSys.SharedKernel.Persistence;
using Finance.Infrastructure.Persistence.DBContext;

namespace Finance.Infrastructure.Extensions
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FinanceDbContext _dbContext;

        public UnitOfWork(FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
