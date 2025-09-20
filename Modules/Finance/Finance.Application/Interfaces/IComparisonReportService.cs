// Finance.Application/Interfaces/IComparisonReportService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Finance.Application.Interfaces
{
    public interface IComparisonReportService
    {
        Task<IReadOnlyList<object>> CompareProfitLossAsync(params int[] years);
        Task<IReadOnlyList<object>> CompareExpensesAsync(params int[] years);
        Task<IReadOnlyList<object>> ComparePurchasesAsync(params int[] years);
        Task<IReadOnlyList<object>> CompareFinanceSummaryAsync(params int[] years);


    }
}
