// ============================================
// File: SharedKernel/Domain/IAggregateRoot.cs
// Namespace: ERPSys.SharedKernel.Domain
// Purpose: Marker interface for DDD aggregate roots
// ============================================

namespace ERPSys.SharedKernel.Domain
{
    /// <summary>
    /// EN: Marker interface to distinguish Aggregate Roots from other entities.
    /// Only Aggregate Roots should have their own repositories.
    /// Example: GLAccount, JournalEntry, Employee.
    /// </summary>
    public interface IAggregateRoot { }
}
