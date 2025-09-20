// ============================================
// File: SharedKernel/Persistence/IUnitOfWork.cs
// Namespace: ERPSys.SharedKernel.Persistence
// Purpose: Unit of Work pattern abstraction
// ============================================

using System.Threading;
using System.Threading.Tasks;

namespace ERPSys.SharedKernel.Persistence
{
    /// <summary>
    /// EN: Unit of Work abstraction to coordinate transactions across repositories.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Save changes in a transaction.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
