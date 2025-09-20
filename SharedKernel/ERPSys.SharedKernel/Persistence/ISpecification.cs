// ============================================
// File: SharedKernel/Persistence/ISpecification.cs
// Namespace: ERPSys.SharedKernel.Persistence
// Purpose: Specification pattern abstraction
// ============================================

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ERPSys.SharedKernel.Persistence
{
    /// <summary>
    /// EN: Specification pattern to encapsulate query logic.
    /// </summary>
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>>? Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        Expression<Func<T, object>>? OrderBy { get; }
        Expression<Func<T, object>>? OrderByDescending { get; }
        int? Take { get; }
        int? Skip { get; }
        bool IsPagingEnabled { get; }
    }
}
