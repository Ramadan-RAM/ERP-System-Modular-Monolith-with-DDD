using Finance.Application.DTOs;

namespace Finance.Application.Interfaces
{
    public interface IGLAccountService
    {
        Task<GLAccountDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<GLAccountDto>> GetAllAsync();
        Task<GLAccountDto> CreateAsync(GLAccountDto dto);
        Task UpdateAsync(GLAccountDto dto);
        Task DeleteAsync(int id);

        // 🔹 Custom
        Task<GLAccountDto?> GetByCodeAsync(string code);
        Task<IReadOnlyList<GLAccountDto>> GetByTypeAsync(string accountType);
    }
}
