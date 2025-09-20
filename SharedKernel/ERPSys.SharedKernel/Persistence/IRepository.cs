// ============================================
// File: SharedKernel/Persistence/IRepository.cs
// Namespace: ERPSys.SharedKernel.Persistence
// Purpose: Abstraction for read-write queries
// ============================================

using System.Threading;
using System.Threading.Tasks;
using ERPSys.SharedKernel.Domain;

namespace ERPSys.SharedKernel.Persistence
{
    /// <summary>
    /// EN: Generic repository for aggregates with CRUD operations.
    /// </summary>
    public interface IRepository<T, TId> : IReadOnlyRepository<T, TId> where T : BaseEntity<TId>, IAggregateRoot
    {
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}
