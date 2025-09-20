// ============================================
// File: SharedKernel/Persistence/IReadOnlyRepository.cs
// Namespace: ERPSys.SharedKernel.Persistence
// Purpose: Abstraction for read-only queries
// ============================================

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERPSys.SharedKernel.Domain;

namespace ERPSys.SharedKernel.Persistence
{
    /// <summary>
    /// EN: Generic read-only repository for aggregates.
    /// </summary>
    public interface IReadOnlyRepository<T, TId> where T : BaseEntity<TId>, IAggregateRoot
    {
        Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
