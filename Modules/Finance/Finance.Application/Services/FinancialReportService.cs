using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Finance.Domain.Entities;
using Finance.Domain.ValueObjects;
using ERPSys.SharedKernel.Persistence;
using Mapster;

namespace Finance.Application.Services
{
    public class FinancialReportService : IFinancialReportService
    {
        private readonly IRepository<GeneralExpense, int> _expenseRepo;
        private readonly IRepository<PurchaseOrder, int> _purchaseRepo;
        private readonly IRepository<ProfitLossForecast, int> _forecastRepo;

        public FinancialReportService(
            IRepository<GeneralExpense, int> expenseRepo,
            IRepository<PurchaseOrder, int> purchaseRepo,
            IRepository<ProfitLossForecast, int> forecastRepo)
        {
            _expenseRepo = expenseRepo;
            _purchaseRepo = purchaseRepo;
            _forecastRepo = forecastRepo;
        }

        public async Task<ProfitLossForecastDto> GetMonthlyReportAsync(int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            return await GenerateProfitLossReportAsync(start, end);
        }

        public async Task<ProfitLossForecastDto> GetYearlyReportAsync(int year)
        {
            var start = new DateTime(year, 1, 1);
            var end = new DateTime(year, 12, 31);
            return await GenerateProfitLossReportAsync(start, end);
        }

        public async Task<IReadOnlyList<GeneralExpenseDto>> GetExpensesReportAsync(DateTime start, DateTime end)
        {
            var expenses = await _expenseRepo.ListAsync(e => e.ExpenseDate >= start && e.ExpenseDate <= end);
            return expenses.Adapt<IReadOnlyList<GeneralExpenseDto>>();
        }

        public async Task<IReadOnlyList<PurchaseOrderDto>> GetPurchasesReportAsync(DateTime start, DateTime end)
        {
            var purchases = await _purchaseRepo.ListAsync(p => p.OrderDate >= start && p.OrderDate <= end);
            return purchases.Adapt<IReadOnlyList<PurchaseOrderDto>>();
        }

        public async Task<MoneyDto> GetNetProfitAsync(DateTime start, DateTime end)
        {
            var forecast = await GenerateProfitLossReportAsync(start, end);

            if (forecast.TotalExpectedProfit.CurrencyCode != forecast.TotalExpectedLoss.CurrencyCode)
                throw new InvalidOperationException("Currency mismatch in profit/loss calculation.");

            return new MoneyDto
            {
                Amount = forecast.TotalExpectedProfit.Amount - forecast.TotalExpectedLoss.Amount,
                CurrencyCode = forecast.TotalExpectedProfit.CurrencyCode
            };
        }


        public async Task<decimal> GetTotalExpensesAsync(DateTime start, DateTime end)
        {
            var expenses = await _expenseRepo.ListAsync(e => e.ExpenseDate >= start && e.ExpenseDate <= end);
            return expenses.Sum(e => e.Amount.Amount);
        }

        public async Task<decimal> GetTotalPurchasesAsync(DateTime start, DateTime end)
        {
            var purchases = await _purchaseRepo.ListAsync(p => p.OrderDate >= start && p.OrderDate <= end);
            return purchases.Sum(p => p.Total.Amount);
        }

        // ✅ Helper Method
        private async Task<ProfitLossForecastDto> GenerateProfitLossReportAsync(DateTime start, DateTime end)
        {
            var expenses = await _expenseRepo.ListAsync(e => e.ExpenseDate >= start && e.ExpenseDate <= end);
            var purchases = await _purchaseRepo.ListAsync(p => p.OrderDate >= start && p.OrderDate <= end);

            var currency = purchases.FirstOrDefault()?.Total.CurrencyCode
                           ?? expenses.FirstOrDefault()?.Amount.CurrencyCode
                           ?? "USD";

            var totalExpenses = expenses.Sum(e => e.Amount.Amount);
            var totalPurchases = purchases.Sum(p => p.Total.Amount);

            var netProfit = totalPurchases - totalExpenses;

            var forecast = new ProfitLossForecast(
                new Period(start, end),
                new Money(netProfit > 0 ? netProfit : 0, currency),
                new Money(netProfit < 0 ? Math.Abs(netProfit) : 0, currency)
            );

            await _forecastRepo.AddAsync(forecast);
            return forecast.Adapt<ProfitLossForecastDto>();
        }
    }
}
