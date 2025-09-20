// Finance.Application/Services/ComparisonReportService.cs
using Finance.Application.Interfaces;
using Finance.Domain.Entities;
using ERPSys.SharedKernel.Persistence;

namespace Finance.Application.Services
{
    public class ComparisonReportService : IComparisonReportService
    {
        private readonly IRepository<ProfitLossForecast, int> _forecastRepo;
        private readonly IRepository<GeneralExpense, int> _expenseRepo;
        private readonly IRepository<PurchaseOrder, int> _purchaseRepo;

        public ComparisonReportService(
            IRepository<ProfitLossForecast, int> forecastRepo,
            IRepository<GeneralExpense, int> expenseRepo,
            IRepository<PurchaseOrder, int> purchaseRepo)
        {
            _forecastRepo = forecastRepo;
            _expenseRepo = expenseRepo;
            _purchaseRepo = purchaseRepo;
        }

        public async Task<IReadOnlyList<object>> CompareProfitLossAsync(params int[] years)
        {
            var forecasts = await _forecastRepo.ListAsync(f => years.Contains(f.Period.Start.Year));

            return forecasts
                .GroupBy(f => f.Period.Start.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    TotalProfit = g.Sum(x => x.TotalExpectedProfit.Amount),
                    TotalLoss = g.Sum(x => x.TotalExpectedLoss.Amount),
                    Net = g.Sum(x => x.TotalExpectedProfit.Amount) - g.Sum(x => x.TotalExpectedLoss.Amount)
                })
                .OrderBy(r => r.Year)
                .ToList();
        }

        public async Task<IReadOnlyList<object>> CompareExpensesAsync(params int[] years)
        {
            var expenses = await _expenseRepo.ListAsync(e => years.Contains(e.ExpenseDate.Year));

            return expenses
                .GroupBy(e => e.ExpenseDate.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    TotalExpenses = g.Sum(x => x.Amount.Amount),
                    Count = g.Count()
                })
                .OrderBy(r => r.Year)
                .ToList();
        }

        public async Task<IReadOnlyList<object>> ComparePurchasesAsync(params int[] years)
        {
            var purchases = await _purchaseRepo.ListAsync(p => years.Contains(p.OrderDate.Year));

            return purchases
                .GroupBy(p => p.OrderDate.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    TotalPurchases = g.Sum(x => x.Total.Amount),
                    OrdersCount = g.Count()
                })
                .OrderBy(r => r.Year)
                .ToList();
        }

        public async Task<IReadOnlyList<object>> CompareFinanceSummaryAsync(params int[] years)
        {
            var profitLoss = await CompareProfitLossAsync(years);
            var expenses = await CompareExpensesAsync(years);
            var purchases = await ComparePurchasesAsync(years);

            var result = years.Select(y => new
            {
                Year = y,
                ProfitLoss = profitLoss.FirstOrDefault(p => (int)p.GetType().GetProperty("Year")!.GetValue(p)! == y),
                Expenses = expenses.FirstOrDefault(e => (int)e.GetType().GetProperty("Year")!.GetValue(e)! == y),
                Purchases = purchases.FirstOrDefault(pu => (int)pu.GetType().GetProperty("Year")!.GetValue(pu)! == y)
            }).ToList();

            return result;
        }
    }
}
