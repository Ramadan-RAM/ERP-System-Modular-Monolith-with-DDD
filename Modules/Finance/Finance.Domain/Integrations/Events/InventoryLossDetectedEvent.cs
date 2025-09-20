// Finance.Domain/Integrations/Events/InventoryLossDetectedEvent.cs
using ERPSys.SharedKernel.Events;

namespace Finance.Domain.Integrations.Events
{
    /// <summary>
    /// From Inventory: Inventory loss. Finance creates an inventory loss entry.
    /// </summary>
    public sealed class InventoryLossDetectedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public string SKU { get; }
        public decimal QuantityLost { get; }
        public DateTime DetectedAt { get; }

        public InventoryLossDetectedEvent(Guid productId, string sku, decimal quantityLost, DateTime detectedAt)
        {
            ProductId = productId;
            SKU = sku;
            QuantityLost = quantityLost;
            DetectedAt = detectedAt;
        }
    }
}
