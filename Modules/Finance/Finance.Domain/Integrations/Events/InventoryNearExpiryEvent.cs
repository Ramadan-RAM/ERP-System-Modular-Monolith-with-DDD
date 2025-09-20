// Finance.Domain/Integrations/Events/InventoryNearExpiryEvent.cs
using ERPSys.SharedKernel.Events;

namespace Finance.Domain.Integrations.Events
{
    /// <summary>
    /// From Inventory: An item is nearing expiration. Finance adjusts forecasts and suggests discounts.
    /// </summary>
    public sealed class InventoryNearExpiryEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public string SKU { get; }
        public int DaysRemaining { get; }
        public decimal Quantity { get; }

        public InventoryNearExpiryEvent(Guid productId, string sku, int daysRemaining, decimal quantity)
        {
            ProductId = productId;
            SKU = sku;
            DaysRemaining = daysRemaining;
            Quantity = quantity;
        }
    }
}
