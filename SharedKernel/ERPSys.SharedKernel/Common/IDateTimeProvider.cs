// ============================================
// File: SharedKernel/Common/IDateTimeProvider.cs
// Namespace: ERPSys.SharedKernel.Common
// Purpose: Abstract current time for testability
// ============================================

using System;

namespace ERPSys.SharedKernel.Common
{
    /// <summary>
    /// EN: Abstracts system time for consistency and testability.
    ///
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
        DateTime Now { get; }
    }
}
