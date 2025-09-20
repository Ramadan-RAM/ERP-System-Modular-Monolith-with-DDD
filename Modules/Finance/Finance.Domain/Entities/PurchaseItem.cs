// Finance.Domain/Entities/PurchaseItem.cs
using ERPSys.SharedKernel.Domain;
using Finance.Domain.Common.Enums;
using Finance.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Domain.Entities
{
    /// <summary>
    /// Purchase order item. Contains a purchase price and (optionally) an expected sale price + expiration information.
    /// </summary>
    public class PurchaseItem : BaseEntity<int>, IAggregateRoot
    {
        public int PurchaseOrderId { get; private set; }
        public PurchaseOrder PurchaseOrder { get; private set; } = default!;

        public Guid ProductId { get; private set; } // From Inventory/CRM
        public string ProductName { get; private set; } = default!;
        public InventoryItemType ItemType { get; private set; }
        public decimal Quantity { get; private set; }
        public Money UnitPrice { get; private set; } = default!;
        public Money? ExpectedSalePrice { get; private set; }

        [NotMapped]
        public ExpiryInfo? Expiry { get; private set; }

        private PurchaseItem() { }

        public PurchaseItem(Guid productId, string productName, InventoryItemType type, decimal quantity, Money unitPrice, Money? expectedSalePrice = null, ExpiryInfo? expiry = null)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be > 0");

            ProductId = productId;
            ProductName = productName.Trim();
            ItemType = type;
            Quantity = quantity;
            UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
            ExpectedSalePrice = expectedSalePrice;
            Expiry = expiry;
        }
    }
}
