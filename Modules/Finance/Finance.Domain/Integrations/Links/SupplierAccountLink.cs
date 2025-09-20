// Finance.Domain/Integrations/Links/SupplierAccountLink.cs
using ERPSys.SharedKernel.Domain;

namespace Finance.Domain.Integrations.Links
{
    /// <summary>
    /// Link a supplier (from CRM/Procurement) to a supplier GL account.
    /// </summary>
    public class SupplierAccountLink : BaseEntity<int>
    {
        public Guid SupplierId { get; private set; }
        public int GLAccountId { get; private set; }
        private SupplierAccountLink() { }
        public SupplierAccountLink(Guid supplierId, int glAccountId)
        {
            SupplierId = supplierId;
            GLAccountId = glAccountId;
        }
    }
}
