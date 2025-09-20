// Finance.Domain/Entities/PurchaseOrder.cs
using ERPSys.SharedKernel.Domain;
using Finance.Domain.Common.Enums;

using Finance.Domain.ValueObjects;

namespace Finance.Domain.Entities
{
    /// <summary>
    /// Purchase order linked to a supplier (SupplierId from CRM/Procurement).
    /// </summary>
    public class PurchaseOrder : BaseEntity<int>, IAggregateRoot
    {
        public DocumentNumber Number { get; private set; } = default!;
        public Guid SupplierId { get; private set; }  //External reference
        public DateTime OrderDate { get; private set; }
        public PurchaseStatus Status { get; private set; } = PurchaseStatus.Draft;

        private readonly List<PurchaseItem> _items = new();
        public IReadOnlyCollection<PurchaseItem> Items => _items.AsReadOnly();

        public Money Total { get; private set; } = default!;

        private PurchaseOrder() { }

        public PurchaseOrder(DocumentNumber number, Guid supplierId, DateTime orderDate, string currency)
        {
            Number = number ?? throw new ArgumentNullException(nameof(number));
            SupplierId = supplierId;
            OrderDate = orderDate.Date;
            Total = new Money(0m, currency);
        }

        public void AddItem(PurchaseItem item)
        {
            _items.Add(item ?? throw new ArgumentNullException(nameof(item)));
            RecalculateTotal();
        }

        public void ChangeStatus(PurchaseStatus status) => Status = status;

        private void RecalculateTotal()
        {
            if (!_items.Any()) return;
            var currency = _items.First().UnitPrice.CurrencyCode;
            var sum = _items.Aggregate(0m, (s, i) => s + (i.UnitPrice.Amount * i.Quantity));
            Total = new Money(sum, currency);
        }
    }
}
