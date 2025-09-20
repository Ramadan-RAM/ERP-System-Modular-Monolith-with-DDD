// ============================================
// File: SharedKernel/Common/PagedList.cs
// Namespace: ERPSys.SharedKernel.Common
// Purpose: Represents paginated query results
// ============================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPSys.SharedKernel.Common
{
    /// <summary>
    /// EN: Paginated result set with metadata (page index, size, total count).
    ///
    /// </summary>
    public class PagedList<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PagedList(IReadOnlyList<T> items, int count, int pageIndex, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public static PagedList<T> Create(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
