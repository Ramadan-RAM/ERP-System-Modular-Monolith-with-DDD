using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Finance.Domain.Entities;
using Finance.Domain.Common.Enums;
using Finance.Domain.ValueObjects;
using ERPSys.SharedKernel.Persistence;
using Mapster;

namespace Finance.Application.Services
{
    public class InventoryForecastService : IInventoryForecastService
    {
        private readonly IRepository<InventoryItemSnapshot, int> _snapshotRepo;
        private readonly IRepository<ProfitLossForecast, int> _forecastRepo;

        public InventoryForecastService(
            IRepository<InventoryItemSnapshot, int> snapshotRepo,
            IRepository<ProfitLossForecast, int> forecastRepo)
        {
            _snapshotRepo = snapshotRepo;
            _forecastRepo = forecastRepo;
        }

        public async Task<ProfitLossForecastDto> GenerateForecastAsync(DateTime start, DateTime end)
        {
            var snapshots = await _snapshotRepo.ListAsync(s => s.AsOf >= start && s.AsOf <= end);

            if (!snapshots.Any())
                throw new InvalidOperationException("No snapshots found for the given period.");

            var currency = snapshots.First().AvgPurchaseCost.CurrencyCode;

            var totalProfit = snapshots.Sum(s => s.ExpectedGrossProfit.Amount);
            var totalLoss = snapshots.Sum(s => s.ExpectedLoss.Amount);

            var forecast = new ProfitLossForecast(
                new Period(start, end),
                new Money(totalProfit, currency),
                new Money(totalLoss, currency)
            );

            await _forecastRepo.AddAsync(forecast);
            return forecast.Adapt<ProfitLossForecastDto>();
        }

        public async Task<IReadOnlyList<InventoryForecastDto>> GetNearExpiryAsync(int thresholdDays)
        {
            var today = DateTime.UtcNow.Date;
            var entities = await _snapshotRepo.ListAsync(s =>
                s.Expiry != null &&
                s.Expiry.DaysToExpire(today).HasValue &&
                s.Expiry.DaysToExpire(today).Value <= thresholdDays);

            return entities.Adapt<IReadOnlyList<InventoryForecastDto>>();
        }

        public async Task<IReadOnlyList<InventoryForecastDto>> GetByStatusAsync(ProfitLossStatus status)
        {
            var entities = await _snapshotRepo.ListAsync(s => s.ForecastStatus == status);
            return entities.Adapt<IReadOnlyList<InventoryForecastDto>>();
        }

        public async Task<IReadOnlyList<InventoryForecastDto>> GetByProductAsync(Guid productId)
        {
            var entities = await _snapshotRepo.ListAsync(s => s.ProductId == productId);
            return entities.Adapt<IReadOnlyList<InventoryForecastDto>>();
        }

        public async Task<IReadOnlyList<InventoryForecastDto>> GetByPeriodAsync(DateTime start, DateTime end)
        {
            var entities = await _snapshotRepo.ListAsync(s => s.AsOf >= start && s.AsOf <= end);
            return entities.Adapt<IReadOnlyList<InventoryForecastDto>>();
        }
    }
}
