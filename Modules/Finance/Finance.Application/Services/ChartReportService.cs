using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Finance.Domain.Entities;
using ERPSys.SharedKernel.Persistence;

namespace Finance.Application.Services
{
    public class ChartReportService : IChartReportService
    {
        private readonly IRepository<ProfitLossForecast, int> _forecastRepo;
        private readonly IRepository<GeneralExpense, int> _expenseRepo;
        private readonly IRepository<PurchaseOrder, int> _purchaseRepo;

        public ChartReportService(
            IRepository<ProfitLossForecast, int> forecastRepo,
            IRepository<GeneralExpense, int> expenseRepo,
            IRepository<PurchaseOrder, int> purchaseRepo)
        {
            _forecastRepo = forecastRepo;
            _expenseRepo = expenseRepo;
            _purchaseRepo = purchaseRepo;
        }

        public async Task<IReadOnlyList<YearlyComparisonDto>> GetMonthlyProfitLossTrendAsync(params int[] years)
        {
            var result = new List<YearlyComparisonDto>();

            foreach (var year in years)
            {
                var forecasts = await _forecastRepo.ListAsync(f =>
                    f.Period.Start.Year == year);

                var monthly = forecasts
                    .GroupBy(f => f.Period.Start.Month)
                    .Select(g => new ChartSeriesDto
                    {
                        Label = $"{g.Key:D2}/{year}",
                        Value = g.Sum(x => x.TotalExpectedProfit.Amount - x.TotalExpectedLoss.Amount)
                    }).ToList();

                result.Add(new YearlyComparisonDto
                {
                    Year = year,
                    Series = monthly
                });
            }

            return result;
        }

        public async Task<IReadOnlyList<ChartSeriesDto>> GetExpenseBreakdownByCategoryAsync(int year)
        {
            var expenses = await _expenseRepo.ListAsync(e => e.ExpenseDate.Year == year);

            return expenses
                .GroupBy(e => e.Category.ToString())
                .Select(g => new ChartSeriesDto
                {
                    Label = g.Key,
                    Value = g.Sum(x => x.Amount.Amount)
                }).ToList();
        }

        public async Task<IReadOnlyList<ChartSeriesDto>> GetPurchasesBySupplierAsync(int year)
        {
            var purchases = await _purchaseRepo.ListAsync(p => p.OrderDate.Year == year);

            return purchases
                .GroupBy(p => p.SupplierId.ToString())
                .Select(g => new ChartSeriesDto
                {
                    Label = $"Supplier-{g.Key.Substring(0, 6)}",
                    Value = g.Sum(x => x.Total.Amount)
                }).ToList();
        }

        public async Task<KpiSummaryDto> GetKpiSummaryAsync(int year)
        {
            var forecasts = await _forecastRepo.ListAsync(f => f.Period.Start.Year == year);
            var expenses = await _expenseRepo.ListAsync(e => e.ExpenseDate.Year == year);

            var netProfit = forecasts.Sum(f => f.TotalExpectedProfit.Amount - f.TotalExpectedLoss.Amount);
            var totalExpenses = expenses.Sum(e => e.Amount.Amount);
            var avgExpensePerMonth = totalExpenses / 12m;

            // Year-on-year comparison compared to the previous year
            var prevYearForecasts = await _forecastRepo.ListAsync(f => f.Period.Start.Year == year - 1);
            var prevNetProfit = prevYearForecasts.Sum(f => f.TotalExpectedProfit.Amount - f.TotalExpectedLoss.Amount);

            decimal yoyGrowth = prevNetProfit == 0 ? 0 : ((netProfit - prevNetProfit) / prevNetProfit) * 100;

            // Net Profit Margin = NetProfit ÷ (NetProfit + Expenses)
            decimal marginBase = netProfit + totalExpenses;
            var netProfitMargin = marginBase == 0 ? 0 : (netProfit / marginBase) * 100;

            return new KpiSummaryDto
            {
                NetProfit = netProfit,
                NetProfitMargin = Math.Round(netProfitMargin, 2),
                TotalExpenses = totalExpenses,
                AvgExpensePerMonth = Math.Round(avgExpensePerMonth, 2),
                YearOverYearGrowth = Math.Round(yoyGrowth, 2)
            };
        }

    }
}
