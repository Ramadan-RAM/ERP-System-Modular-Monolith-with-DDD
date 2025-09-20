using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Finance.Domain.Entities;
using ERPSys.SharedKernel.Persistence;
using Mapster;

namespace Finance.Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IRepository<GeneralExpense, int> _expenseRepository;

        public ExpenseService(IRepository<GeneralExpense, int> expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public async Task<GeneralExpenseDto?> GetByIdAsync(int id)
        {
            var entity = await _expenseRepository.GetByIdAsync(id);
            return entity?.Adapt<GeneralExpenseDto>();
        }

        public async Task<IReadOnlyList<GeneralExpenseDto>> GetAllAsync()
        {
            var entities = await _expenseRepository.ListAllAsync();
            return entities.Adapt<IReadOnlyList<GeneralExpenseDto>>();
        }

        public async Task<GeneralExpenseDto> CreateAsync(GeneralExpenseDto dto)
        {
            var entity = dto.Adapt<GeneralExpense>();
            await _expenseRepository.AddAsync(entity);
            return entity.Adapt<GeneralExpenseDto>();
        }

        public async Task UpdateAsync(GeneralExpenseDto dto)
        {
            var entity = await _expenseRepository.GetByIdAsync(dto.Id);
            if (entity == null) throw new KeyNotFoundException("Expense not found");

            // We adapt the loaded entity to ensure correct tracking from EF
            dto.Adapt(entity);
            await _expenseRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _expenseRepository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Expense not found");

            await _expenseRepository.DeleteAsync(entity);
        }

        // ✅ Custom Methods
        public async Task<IReadOnlyList<GeneralExpenseDto>> GetByPeriodAsync(DateTime start, DateTime end)
        {
            var s = start.Date; var e = end.Date;
            var expenses = await _expenseRepository.ListAsync(x => x.ExpenseDate >= s && x.ExpenseDate <= e);
            return expenses.Adapt<IReadOnlyList<GeneralExpenseDto>>();
        }

        public async Task<decimal> GetTotalByCategoryAsync(string category, DateTime start, DateTime end)
        {
            var s = start.Date; var e = end.Date;
            var items = await _expenseRepository.ListAsync(x =>
                x.ExpenseDate >= s && x.ExpenseDate <= e &&
                x.Category.ToString() == category);

            return items.Sum(x => x.Amount.Amount);
        }

        public async Task<IReadOnlyList<GeneralExpenseDto>> GetByDepartmentAsync(Guid departmentId, DateTime start, DateTime end)
        {
            var s = start.Date; var e = end.Date;
            var items = await _expenseRepository.ListAsync(x =>
                x.DepartmentId == departmentId &&
                x.ExpenseDate >= s && x.ExpenseDate <= e);

            return items.Adapt<IReadOnlyList<GeneralExpenseDto>>();
        }
    }
}
