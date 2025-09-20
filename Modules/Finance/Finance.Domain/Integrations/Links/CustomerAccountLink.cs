// Finance.Domain/Integrations/Links/CustomerAccountLink.cs
using ERPSys.SharedKernel.Domain;

namespace Finance.Domain.Integrations.Links
{
    /// <summary>
    /// Link a customer (from CRM/Users) to a GL account to receive/post customer entries.
    /// </summary>
    public class CustomerAccountLink : BaseEntity<int>
    {
        public Guid CustomerId { get; private set; } // From CRM/Users
        public int GLAccountId { get; private set; } // Link to account
        private CustomerAccountLink() { }
        public CustomerAccountLink(Guid customerId, int glAccountId)
        {
            CustomerId = customerId;
            GLAccountId = glAccountId;
        }
    }
}