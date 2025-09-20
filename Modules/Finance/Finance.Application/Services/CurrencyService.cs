using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Finance.Domain.Entities;
using ERPSys.SharedKernel.Persistence;
using Mapster;

namespace Finance.Application.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IRepository<Currency, int> _repo;

        public CurrencyService(IRepository<Currency, int> repo)
        {
            _repo = repo;
        }

        public async Task<CurrencyDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity?.Adapt<CurrencyDto>();
        }

        public async Task<IReadOnlyList<CurrencyDto>> GetAllAsync()
        {
            var entities = await _repo.ListAllAsync();
            return entities.Adapt<IReadOnlyList<CurrencyDto>>();
        }

        public async Task<CurrencyDto> CreateAsync(CurrencyDto dto)
        {
            var entity = dto.Adapt<Currency>();
            await _repo.AddAsync(entity);
            return entity.Adapt<CurrencyDto>();
        }

        public async Task UpdateAsync(CurrencyDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null) throw new KeyNotFoundException("Currency not found");

            dto.Adapt(entity);
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Currency not found");

            await _repo.DeleteAsync(entity);
        }

        // 🔹 Custom
        public async Task<CurrencyDto?> GetByCodeAsync(string code)
        {
            var entities = await _repo.ListAsync(c => c.Code == code.ToUpperInvariant());
            return entities.FirstOrDefault()?.Adapt<CurrencyDto>();
        }

        public async Task<CurrencyDto?> GetBaseCurrencyAsync()
        {
            var entities = await _repo.ListAsync(c => c.IsBase);
            return entities.FirstOrDefault()?.Adapt<CurrencyDto>();
        }
    }
}
