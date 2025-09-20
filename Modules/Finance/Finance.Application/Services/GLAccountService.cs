using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Finance.Domain.Entities;
using Finance.Domain.Common.Enums;
using ERPSys.SharedKernel.Persistence;
using Mapster;

namespace Finance.Application.Services
{
    public class GLAccountService : IGLAccountService
    {
        private readonly IRepository<GLAccount, int> _repo;

        public GLAccountService(IRepository<GLAccount, int> repo)
        {
            _repo = repo;
        }

        public async Task<GLAccountDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity?.Adapt<GLAccountDto>();
        }

        public async Task<IReadOnlyList<GLAccountDto>> GetAllAsync()
        {
            var entities = await _repo.ListAllAsync();
            return entities.Adapt<IReadOnlyList<GLAccountDto>>();
        }

        public async Task<GLAccountDto> CreateAsync(GLAccountDto dto)
        {
            var entity = dto.Adapt<GLAccount>();
            await _repo.AddAsync(entity);
            return entity.Adapt<GLAccountDto>();
        }

        public async Task UpdateAsync(GLAccountDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new KeyNotFoundException("GLAccount not found");

            dto.Adapt(entity); // Update values ​​only without creating a new Entity
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("GLAccount not found");

            await _repo.DeleteAsync(entity);
        }

        // 🔹 Custom
        public async Task<GLAccountDto?> GetByCodeAsync(string code)
        {
            var accounts = await _repo.ListAsync(a => a.Code.Value == code);
            return accounts.FirstOrDefault()?.Adapt<GLAccountDto>();
        }

        public async Task<IReadOnlyList<GLAccountDto>> GetByTypeAsync(string accountType)
        {
            if (!Enum.TryParse<AccountType>(accountType, true, out var type))
                throw new ArgumentException("Invalid account type");

            var accounts = await _repo.ListAsync(a => a.Type == type);
            return accounts.Adapt<IReadOnlyList<GLAccountDto>>();
        }
    }
}
