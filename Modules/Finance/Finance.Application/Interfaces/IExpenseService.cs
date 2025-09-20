using Finance.Application.DTOs;

namespace Finance.Application.Interfaces
{
    public interface IExpenseService
    {
        Task<GeneralExpenseDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<GeneralExpenseDto>> GetAllAsync();
        Task<GeneralExpenseDto> CreateAsync(GeneralExpenseDto dto);
        Task UpdateAsync(GeneralExpenseDto dto);
        Task DeleteAsync(int id);

        // ✅ Custom Methods
        Task<IReadOnlyList<GeneralExpenseDto>> GetByPeriodAsync(DateTime start, DateTime end);
        Task<decimal> GetTotalByCategoryAsync(string category, DateTime start, DateTime end);
        Task<IReadOnlyList<GeneralExpenseDto>> GetByDepartmentAsync(Guid departmentId, DateTime start, DateTime end);
    }
}
