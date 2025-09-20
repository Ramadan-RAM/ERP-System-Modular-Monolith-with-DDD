using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Finance.Domain.Entities;
using Finance.Domain.Common.Enums;
using ERPSys.SharedKernel.Persistence;
using Mapster;

namespace Finance.Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IRepository<PurchaseOrder, int> _orderRepo;

        public PurchaseService(IRepository<PurchaseOrder, int> orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<PurchaseOrderDto?> GetByIdAsync(int id)
        {
            var entity = await _orderRepo.GetByIdAsync(id);
            return entity?.Adapt<PurchaseOrderDto>();
        }

        public async Task<IReadOnlyList<PurchaseOrderDto>> GetAllAsync()
        {
            var entities = await _orderRepo.ListAllAsync();
            return entities.Adapt<IReadOnlyList<PurchaseOrderDto>>();
        }

        public async Task<PurchaseOrderDto> CreateAsync(PurchaseOrderDto dto)
        {
            var entity = dto.Adapt<PurchaseOrder>();
            await _orderRepo.AddAsync(entity);
            return entity.Adapt<PurchaseOrderDto>();
        }

        public async Task UpdateAsync(PurchaseOrderDto dto)
        {
            var entity = await _orderRepo.GetByIdAsync(dto.Id);
            if (entity == null) throw new KeyNotFoundException("Purchase order not found");

            dto.Adapt(entity);
            await _orderRepo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _orderRepo.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Purchase order not found");

            await _orderRepo.DeleteAsync(entity);
        }

        // ✅ Custom
        public async Task AddItemAsync(int orderId, PurchaseItemDto itemDto)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Purchase order not found");

            var item = itemDto.Adapt<PurchaseItem>();
            order.AddItem(item);
            await _orderRepo.UpdateAsync(order);
        }

        public async Task<IReadOnlyList<PurchaseOrderDto>> GetBySupplierAsync(Guid supplierId)
        {
            var entities = await _orderRepo.ListAsync(o => o.SupplierId == supplierId);
            return entities.Adapt<IReadOnlyList<PurchaseOrderDto>>();
        }

        public async Task<IReadOnlyList<PurchaseOrderDto>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            var entities = await _orderRepo.ListAsync(o => o.OrderDate >= start && o.OrderDate <= end);
            return entities.Adapt<IReadOnlyList<PurchaseOrderDto>>();
        }

        public async Task<IReadOnlyList<PurchaseOrderDto>> GetByStatusAsync(PurchaseStatus status)
        {
            var entities = await _orderRepo.ListAsync(o => o.Status == status);
            return entities.Adapt<IReadOnlyList<PurchaseOrderDto>>();
        }

        public async Task<decimal> GetTotalOrdersAmountAsync(Guid supplierId, DateTime start, DateTime end)
        {
            var orders = await _orderRepo.ListAsync(o =>
                o.SupplierId == supplierId &&
                o.OrderDate >= start &&
                o.OrderDate <= end);

            return orders.Sum(o => o.Total.Amount);
        }
    }
}
