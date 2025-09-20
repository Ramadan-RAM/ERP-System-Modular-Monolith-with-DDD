using ERPSys.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ERPSys.SharedKernel.Persistence
{
    public class EfRepository<TContext, T, TId> : IRepository<T, TId>
        where TContext : DbContext
        where T : BaseEntity<TId>, IAggregateRoot
    {
        protected readonly TContext _context;

        public EfRepository(TContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default) =>
            await _context.Set<T>().FindAsync(new object[] { id! }, cancellationToken);

        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default) =>
            await _context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
            await _context.Set<T>().Where(predicate).AsNoTracking().ToListAsync(cancellationToken);

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
            await _context.Set<T>().AnyAsync(predicate, cancellationToken);

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
            await _context.Set<T>().CountAsync(predicate, cancellationToken);

        public IQueryable<T> Query(Expression<Func<T, bool>>? predicate = null) =>
            predicate == null
                ? _context.Set<T>()
                : _context.Set<T>().Where(predicate);

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
            return entity;
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }
    }
}
