using Finance.Application.DTOs;

public interface IFinancialReportService
{
   
    Task<ProfitLossForecastDto> GetMonthlyReportAsync(int year, int month);
    Task<ProfitLossForecastDto> GetYearlyReportAsync(int year);

    Task<IReadOnlyList<GeneralExpenseDto>> GetExpensesReportAsync(DateTime start, DateTime end);
    Task<IReadOnlyList<PurchaseOrderDto>> GetPurchasesReportAsync(DateTime start, DateTime end);

    // ✅ KPI / Summaries
    Task<MoneyDto> GetNetProfitAsync(DateTime start, DateTime end);
    Task<decimal> GetTotalExpensesAsync(DateTime start, DateTime end);
    Task<decimal> GetTotalPurchasesAsync(DateTime start, DateTime end);
}
