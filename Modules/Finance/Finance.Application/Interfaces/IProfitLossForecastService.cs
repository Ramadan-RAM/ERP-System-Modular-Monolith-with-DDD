using Finance.Application.DTOs;

namespace Finance.Application.Interfaces
{
    public interface IProfitLossForecastService
    {
        Task<ProfitLossForecastDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<ProfitLossForecastDto>> GetAllAsync();
        Task<ProfitLossForecastDto> CreateAsync(ProfitLossForecastDto dto);
        Task UpdateAsync(ProfitLossForecastDto dto);
        Task DeleteAsync(int id);

        // ✅ Custom
        Task<ProfitLossForecastDto> GenerateForecastAsync(DateTime start, DateTime end);
        Task<IReadOnlyList<ProfitLossForecastDto>> GetByPeriodAsync(DateTime start, DateTime end);
    }
}
