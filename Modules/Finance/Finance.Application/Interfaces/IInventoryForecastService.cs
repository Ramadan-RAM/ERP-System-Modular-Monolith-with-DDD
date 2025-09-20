using Finance.Application.DTOs;
using Finance.Domain.Common.Enums;

namespace Finance.Application.Interfaces
{
    public interface IInventoryForecastService
    {
        Task<ProfitLossForecastDto> GenerateForecastAsync(DateTime start, DateTime end);

        // Queries
        Task<IReadOnlyList<InventoryForecastDto>> GetNearExpiryAsync(int thresholdDays);
        Task<IReadOnlyList<InventoryForecastDto>> GetByStatusAsync(ProfitLossStatus status);
        Task<IReadOnlyList<InventoryForecastDto>> GetByProductAsync(Guid productId);
        Task<IReadOnlyList<InventoryForecastDto>> GetByPeriodAsync(DateTime start, DateTime end);
    }
}
