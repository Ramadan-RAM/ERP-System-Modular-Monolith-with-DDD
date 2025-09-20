using Finance.Application.DTOs;
using Finance.Application.Interfaces;

using Finance.Domain.Entities;
using ERPSys.SharedKernel.Persistence;
using Mapster;
using Finance.Domain.ValueObjects;

namespace Finance.Application.Services
{
    public class ProfitLossForecastService : IProfitLossForecastService
    {
        private readonly IRepository<ProfitLossForecast, int> _forecastRepo;
        private readonly IRepository<InventoryItemSnapshot, int> _snapshotRepo;
        private readonly IRepository<GeneralExpense, int> _expenseRepo;

        public ProfitLossForecastService(
            IRepository<ProfitLossForecast, int> forecastRepo,
            IRepository<InventoryItemSnapshot, int> snapshotRepo,
            IRepository<GeneralExpense, int> expenseRepo)
        {
            _forecastRepo = forecastRepo;
            _snapshotRepo = snapshotRepo;
            _expenseRepo = expenseRepo;
        }

        public async Task<ProfitLossForecastDto?> GetByIdAsync(int id)
        {
            var entity = await _forecastRepo.GetByIdAsync(id);
            return entity?.Adapt<ProfitLossForecastDto>();
        }

        public async Task<IReadOnlyList<ProfitLossForecastDto>> GetAllAsync()
        {
            var entities = await _forecastRepo.ListAllAsync();
            return entities.Adapt<IReadOnlyList<ProfitLossForecastDto>>();
        }

        public async Task<ProfitLossForecastDto> CreateAsync(ProfitLossForecastDto dto)
        {
            var entity = dto.Adapt<ProfitLossForecast>();
            await _forecastRepo.AddAsync(entity);
            return entity.Adapt<ProfitLossForecastDto>();
        }

        public async Task UpdateAsync(ProfitLossForecastDto dto)
        {
            var entity = await _forecastRepo.GetByIdAsync(dto.Id);
            if (entity == null) throw new KeyNotFoundException("Forecast not found");

            dto.Adapt(entity);
            await _forecastRepo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _forecastRepo.GetByIdAsync(id);
            if (entity != null)
                await _forecastRepo.DeleteAsync(entity);
        }

        // ✅ Real Business Logic
        public async Task<ProfitLossForecastDto> GenerateForecastAsync(DateTime start, DateTime end)
        {
            var snapshots = await _snapshotRepo.ListAsync(s => s.AsOf >= start && s.AsOf <= end);
            var expenses = await _expenseRepo.ListAsync(e => e.ExpenseDate >= start && e.ExpenseDate <= end);

            if (!snapshots.Any() && !expenses.Any())
                throw new InvalidOperationException("No financial data available for the given period.");

            var currency = snapshots.FirstOrDefault()?.AvgPurchaseCost.CurrencyCode
                        ?? expenses.FirstOrDefault()?.Amount.CurrencyCode
                        ?? "USD";

            // ✅ Rely on Domain Logic
            var totalProfit = snapshots.Sum(s => s.ExpectedGrossProfit.Amount);
            var totalLoss = snapshots.Sum(s => s.ExpectedLoss.Amount);
            var totalExpenses = expenses.Sum(e => e.Amount.Amount);

            // 📊 Net profit
            var netProfit = totalProfit - totalLoss - totalExpenses;

            var forecast = new ProfitLossForecast(
                new Period(start, end),
                new Money(netProfit > 0 ? netProfit : 0, currency),
                new Money(netProfit < 0 ? Math.Abs(netProfit) : 0, currency)
            );

            await _forecastRepo.AddAsync(forecast);
            return forecast.Adapt<ProfitLossForecastDto>();
        }


        public async Task<IReadOnlyList<ProfitLossForecastDto>> GetByPeriodAsync(DateTime start, DateTime end)
        {
            var entities = await _forecastRepo.ListAsync(f =>
                f.Period.Start >= start && f.Period.End <= end);

            return entities.Adapt<IReadOnlyList<ProfitLossForecastDto>>();
        }
    }
}
