using Finance.Application.DTOs;
using Finance.Domain.Common.Enums;

namespace Finance.Application.Interfaces
{
    public interface IPurchaseService
    {
        Task<PurchaseOrderDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<PurchaseOrderDto>> GetAllAsync();
        Task<PurchaseOrderDto> CreateAsync(PurchaseOrderDto dto);
        Task UpdateAsync(PurchaseOrderDto dto);
        Task DeleteAsync(int id);

        // ✅ Custom
        Task AddItemAsync(int orderId, PurchaseItemDto itemDto);
        Task<IReadOnlyList<PurchaseOrderDto>> GetBySupplierAsync(Guid supplierId);
        Task<IReadOnlyList<PurchaseOrderDto>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task<IReadOnlyList<PurchaseOrderDto>> GetByStatusAsync(PurchaseStatus status);
        Task<decimal> GetTotalOrdersAmountAsync(Guid supplierId, DateTime start, DateTime end); // ⬅️ الميثود الناقصة
    }
}
