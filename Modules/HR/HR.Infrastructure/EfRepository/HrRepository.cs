using ERPSys.SharedKernel.Domain;
using ERPSys.SharedKernel.Persistence;
using HR.Infrastructure.HR.DbContext;

namespace HR.Infrastructure.EfRepository
{
    public class HrRepository<T, TId> : EfRepository<HRDbContext, T, TId>, IRepository<T, TId>
        where T : BaseEntity<TId>, IAggregateRoot
    {
        public HrRepository(HRDbContext context) : base(context) { }
    }
}
