using Finance.Application.DTOs;

namespace Finance.Application.Interfaces
{
    public interface IInventorySnapshotService
    {
        Task<InventorySnapshotDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<InventorySnapshotDto>> GetAllAsync();
        Task<InventorySnapshotDto> CreateAsync(InventorySnapshotDto dto);
        Task UpdateAsync(InventorySnapshotDto dto);
        Task DeleteAsync(int id);

        // ✅ Custom
        Task<IReadOnlyList<InventorySnapshotDto>> GetNearExpiryAsync(int thresholdDays);
        Task<IReadOnlyList<InventorySnapshotDto>> GetByProductAsync(Guid productId);
        Task<IReadOnlyList<InventorySnapshotDto>> GetByPeriodAsync(DateTime start, DateTime end);
    }
}
