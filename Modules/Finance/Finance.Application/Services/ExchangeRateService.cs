using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Finance.Domain.Entities;
using ERPSys.SharedKernel.Persistence;
using Mapster;

namespace Finance.Application.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IRepository<ExchangeRate, int> _repo;

        public ExchangeRateService(IRepository<ExchangeRate , int> repo)
        {
            _repo = repo;
        }

        public async Task<ExchangeRateDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity?.Adapt<ExchangeRateDto>();
        }

        public async Task<IReadOnlyList<ExchangeRateDto>> GetAllAsync()
        {
            var entities = await _repo.ListAllAsync();
            return entities.Adapt<IReadOnlyList<ExchangeRateDto>>();
        }

        public async Task<ExchangeRateDto> CreateAsync(ExchangeRateDto dto)
        {
            var entity = dto.Adapt<ExchangeRate>();
            await _repo.AddAsync(entity);
            return entity.Adapt<ExchangeRateDto>();
        }

        public async Task UpdateAsync(ExchangeRateDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null) throw new KeyNotFoundException("Exchange rate not found");

            dto.Adapt(entity);
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Exchange rate not found");

            await _repo.DeleteAsync(entity);
        }

        // 🔹 Custom
        public async Task<ExchangeRateDto?> GetRateAsync(string from, string to, DateTime date)
        {
            var entities = await _repo.ListAsync(r =>
                r.FromCurrency == from.ToUpperInvariant() &&
                r.ToCurrency == to.ToUpperInvariant() &&
                r.RateDate.Date == date.Date);

            return entities.FirstOrDefault()?.Adapt<ExchangeRateDto>();
        }

        public async Task<IReadOnlyList<ExchangeRateDto>> GetHistoryAsync(string from, string to)
        {
            var entities = await _repo.ListAsync(r =>
                r.FromCurrency == from.ToUpperInvariant() &&
                r.ToCurrency == to.ToUpperInvariant());

            return entities.Adapt<IReadOnlyList<ExchangeRateDto>>();
        }

        public async Task<ExchangeRateDto?> GetLatestRateAsync(string fromCurrency, string toCurrency)
        {
            var entities = await _repo.ListAsync(r =>
                r.FromCurrency == fromCurrency.ToUpperInvariant() &&
                r.ToCurrency == toCurrency.ToUpperInvariant());

            var latest = entities
                .OrderByDescending(r => r.RateDate)
                .FirstOrDefault();

            return latest?.Adapt<ExchangeRateDto>();
        }

        public async Task<IReadOnlyList<ExchangeRateDto>> GetByDateAsync(DateTime date)
        {
            var entities = await _repo.ListAsync(r => r.RateDate.Date == date.Date);
            return entities.Adapt<IReadOnlyList<ExchangeRateDto>>();
        }

        public async Task<IReadOnlyList<ExchangeRateDto>> GetBetweenDatesAsync(string from, string to, DateTime start, DateTime end)
        {
            var entities = await _repo.ListAsync(r =>
                r.FromCurrency == from.ToUpperInvariant() &&
                r.ToCurrency == to.ToUpperInvariant() &&
                r.RateDate.Date >= start.Date &&
                r.RateDate.Date <= end.Date);

            return entities
                .OrderBy(r => r.RateDate)
                .Adapt<IReadOnlyList<ExchangeRateDto>>();
        }
    }
}
