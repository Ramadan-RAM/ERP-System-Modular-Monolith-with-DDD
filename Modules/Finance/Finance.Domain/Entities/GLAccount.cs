// Finance.Domain/Entities/GLAccount.cs
using ERPSys.SharedKernel.Domain;
using Finance.Domain.Common.Enums;
using Finance.Domain.ValueObjects;
namespace Finance.Domain.Entities
{
    /// <summary>
    /// Accounting guide account. Later linked to supplier/customer/cost center via Links.
    /// </summary>
    public class GLAccount : BaseEntity<int>, IAggregateRoot
    {
      
        public AccountCode Code { get;  set; } = default!;
        public string Name { get;    set; } = default!;
        public AccountType Type { get;   set; }
        public BalanceSide BalanceSide { get;    set; }
        public bool IsActive { get; set; } = true;

        public int? ParentAccountId { get; set; }
        public GLAccount? ParentAccount { get; set; }

        private GLAccount() { } // EF

        public GLAccount(AccountCode code, string name, AccountType type, BalanceSide side, int? parentId = null)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("name required") : name.Trim();
            Type = type;
            BalanceSide = side;
            ParentAccountId = parentId;
        }

      

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
