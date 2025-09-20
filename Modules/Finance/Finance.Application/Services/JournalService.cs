using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Finance.Domain.Entities;
using ERPSys.SharedKernel.Persistence;
using Mapster;

namespace Finance.Application.Services
{
    public class JournalService : IJournalService
    {
        private readonly IRepository<JournalEntry, int> _repo;

        public JournalService(IRepository<JournalEntry, int> repo)
        {
            _repo = repo;
        }

        public async Task<JournalEntryDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity?.Adapt<JournalEntryDto>();
        }

        public async Task<IReadOnlyList<JournalEntryDto>> GetAllAsync()
        {
            var entities = await _repo.ListAllAsync();
            return entities.Adapt<IReadOnlyList<JournalEntryDto>>();
        }

        public async Task<JournalEntryDto> CreateAsync(JournalEntryDto dto)
        {
            var entity = dto.Adapt<JournalEntry>();
            await _repo.AddAsync(entity);
            return entity.Adapt<JournalEntryDto>();
        }

        public async Task UpdateAsync(JournalEntryDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null) throw new KeyNotFoundException("Journal entry not found");

            dto.Adapt(entity);
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Journal entry not found");

            await _repo.DeleteAsync(entity);
        }

        // 🔹 Custom
        public async Task<bool> ApproveAsync(int id)
        {
            var entry = await _repo.GetByIdAsync(id);
            if (entry == null) return false;

            entry.Approve();
            await _repo.UpdateAsync(entry);
            return true;
        }

        public async Task<bool> PostAsync(int id)
        {
            var entry = await _repo.GetByIdAsync(id);
            if (entry == null) return false;

            entry.Post();
            await _repo.UpdateAsync(entry);
            return true;
        }

        public async Task<IReadOnlyList<JournalEntryDto>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            var entries = await _repo.ListAsync(j => j.EntryDate >= start && j.EntryDate <= end);
            return entries.Adapt<IReadOnlyList<JournalEntryDto>>();
        }
    }
}
