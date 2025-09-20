using Finance.Application.DTOs;

namespace Finance.Application.Interfaces
{
    public interface IJournalService
    {
        Task<JournalEntryDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<JournalEntryDto>> GetAllAsync();
        Task<JournalEntryDto> CreateAsync(JournalEntryDto dto);
        Task UpdateAsync(JournalEntryDto dto);
        Task DeleteAsync(int id);

        // 🔹 Custom
        Task<bool> ApproveAsync(int id);
        Task<bool> PostAsync(int id);
        Task<IReadOnlyList<JournalEntryDto>> GetByDateRangeAsync(DateTime start, DateTime end);
    }
}
