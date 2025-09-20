// ============================================
// File: SharedKernel/Persistence/SpecificationEvaluator.cs
// Namespace: ERPSys.SharedKernel.Persistence
// Purpose: Evaluate specification against IQueryable
// ============================================

using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ERPSys.SharedKernel.Persistence
{
    /// <summary>
    /// EN: Evaluates specification and applies it on IQueryable.
    /// </summary>
    public static class SpecificationEvaluator
    {
        public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T> spec) where T : class
        {
            var query = inputQuery;

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);

            if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if (spec.IsPagingEnabled)
                query = query.Skip(spec.Skip!.Value).Take(spec.Take!.Value);

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}
