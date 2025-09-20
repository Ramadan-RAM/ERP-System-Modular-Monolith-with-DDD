using Finance.Application.DTOs;

namespace Finance.Application.Interfaces
{
    public interface IChartReportService
    {
        Task<IReadOnlyList<YearlyComparisonDto>> GetMonthlyProfitLossTrendAsync(params int[] years);
        Task<IReadOnlyList<ChartSeriesDto>> GetExpenseBreakdownByCategoryAsync(int year);
        Task<IReadOnlyList<ChartSeriesDto>> GetPurchasesBySupplierAsync(int year);

        // ✅ KPIs
        Task<KpiSummaryDto> GetKpiSummaryAsync(int year);
    }
}
