using Finance.Application.DTOs;

namespace Finance.Application.Interfaces
{
    public interface ICurrencyService
    {
        Task<CurrencyDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<CurrencyDto>> GetAllAsync();
        Task<CurrencyDto> CreateAsync(CurrencyDto dto);
        Task UpdateAsync(CurrencyDto dto);
        Task DeleteAsync(int id);

        // ✅ Custom
        Task<CurrencyDto?> GetByCodeAsync(string code);
        Task<CurrencyDto?> GetBaseCurrencyAsync(); 
    }
}
