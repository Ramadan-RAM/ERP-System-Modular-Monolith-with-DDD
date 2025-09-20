using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Finance.Domain.Entities;
using ERPSys.SharedKernel.Persistence;
using Mapster;

namespace Finance.Application.Services
{
    public class InventorySnapshotService : IInventorySnapshotService
    {
        private readonly IRepository<InventoryItemSnapshot, int> _repo;

        public InventorySnapshotService(IRepository<InventoryItemSnapshot, int> repo)
        {
            _repo = repo;
        }

        public async Task<InventorySnapshotDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity?.Adapt<InventorySnapshotDto>();
        }

        public async Task<IReadOnlyList<InventorySnapshotDto>> GetAllAsync()
        {
            var entities = await _repo.ListAllAsync();
            return entities.Adapt<IReadOnlyList<InventorySnapshotDto>>();
        }

        public async Task<InventorySnapshotDto> CreateAsync(InventorySnapshotDto dto)
        {
            var entity = dto.Adapt<InventoryItemSnapshot>();
            entity.Recalculate();
            await _repo.AddAsync(entity);
            return entity.Adapt<InventorySnapshotDto>();
        }

        public async Task UpdateAsync(InventorySnapshotDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null) throw new KeyNotFoundException("Snapshot not found");

            dto.Adapt(entity);
            entity.Recalculate();
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
                await _repo.DeleteAsync(entity);
        }

        public async Task<IReadOnlyList<InventorySnapshotDto>> GetNearExpiryAsync(int thresholdDays)
        {
            var today = DateTime.UtcNow.Date;
            var entities = await _repo.ListAsync(s =>
                s.Expiry != null &&
                s.Expiry.DaysToExpire(today).HasValue &&
                s.Expiry.DaysToExpire(today).Value <= thresholdDays);

            return entities.Adapt<IReadOnlyList<InventorySnapshotDto>>();
        }

        public async Task<IReadOnlyList<InventorySnapshotDto>> GetByProductAsync(Guid productId)
        {
            var entities = await _repo.ListAsync(s => s.ProductId == productId);
            return entities.Adapt<IReadOnlyList<InventorySnapshotDto>>();
        }

        public async Task<IReadOnlyList<InventorySnapshotDto>> GetByPeriodAsync(DateTime start, DateTime end)
        {
            var entities = await _repo.ListAsync(s => s.AsOf >= start && s.AsOf <= end);
            return entities.Adapt<IReadOnlyList<InventorySnapshotDto>>();
        }
    }
}
