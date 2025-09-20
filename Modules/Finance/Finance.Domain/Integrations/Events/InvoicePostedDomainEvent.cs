// Finance.Domain/Integrations/Events/InvoicePostedDomainEvent.cs
using ERPSys.SharedKernel.Events;
using Finance.Domain.ValueObjects;

namespace CRM.Domain.Integrations.Events
{
    /// <summary>
    /// Fired when a sales/purchase invoice is posted from CRM/Procurement.
    /// Finance consumes the event to build the constraints.
    /// </summary>
    public sealed class InvoicePostedDomainEvent : DomainEvent
    {
        public Guid InvoiceId { get; }
        public Guid CustomerOrSupplierId { get; }
        public Money Total { get; }
        public DateTime PostedAt { get; }
        public bool IsSales { get; }

        public InvoicePostedDomainEvent(Guid invoiceId, Guid partyId, Money total, bool isSales, DateTime postedAt)
        {
            InvoiceId = invoiceId;
            CustomerOrSupplierId = partyId;
            Total = total;
            IsSales = isSales;
            PostedAt = postedAt;
        }
    }
}
