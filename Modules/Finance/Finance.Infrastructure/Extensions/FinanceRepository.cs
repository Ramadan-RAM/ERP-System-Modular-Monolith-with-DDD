using ERPSys.SharedKernel.Domain;
using ERPSys.SharedKernel.Persistence;
using Finance.Infrastructure.Persistence.DBContext;

namespace Finance.Infrastructure.Persistence
{
    public class FinanceRepository<T, TId> : EfRepository<FinanceDbContext, T, TId>, IRepository<T, TId>
        where T : BaseEntity<TId>, IAggregateRoot
    {
        public FinanceRepository(FinanceDbContext context) : base(context) { }
    }
}
