using Finance.Application.DTOs;

namespace Finance.Application.Interfaces
{
    public interface IExchangeRateService
    {
        Task<ExchangeRateDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<ExchangeRateDto>> GetAllAsync();
        Task<ExchangeRateDto> CreateAsync(ExchangeRateDto dto);
        Task UpdateAsync(ExchangeRateDto dto);
        Task DeleteAsync(int id);

        // ✅ Custom
        Task<ExchangeRateDto?> GetRateAsync(string from, string to, DateTime date);
        Task<IReadOnlyList<ExchangeRateDto>> GetHistoryAsync(string from, string to);
        Task<ExchangeRateDto?> GetLatestRateAsync(string fromCurrency, string toCurrency);
        Task<IReadOnlyList<ExchangeRateDto>> GetByDateAsync(DateTime date);
        Task<IReadOnlyList<ExchangeRateDto>> GetBetweenDatesAsync(string from, string to, DateTime start, DateTime end);
    }
}
