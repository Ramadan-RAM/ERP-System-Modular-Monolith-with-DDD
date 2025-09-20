// Finance.Domain/Entities/GeneralExpense.cs
using ERPSys.SharedKernel.Domain;
using Finance.Domain.Common.Enums;
using Finance.Domain.ValueObjects;

namespace Finance.Domain.Entities
{
    /// <summary>
    /// Daily/monthly general expense associated with a specific cost center and department.
    /// Automatically generates entries according to company policy.
    /// </summary>
    public class GeneralExpense : BaseEntity<int>, IAggregateRoot
    {
        public DateTime ExpenseDate { get; private set; }
        public ExpenseCategory Category { get; private set; }
        public Money Amount { get; private set; } = default!;
        public string? Description { get; private set; }

        public int? CostCenterId { get; private set; }
        public CostCenter? CostCenter { get; private set; }

        public Guid? DepartmentId { get; private set; } // From HR/Users 
        public DateTime Date { get; set; }

        private GeneralExpense() { }

        public GeneralExpense(DateTime date, ExpenseCategory category, Money amount, int? costCenterId = null, Guid? departmentId = null, string? description = null)
        {
            ExpenseDate = date.Date;
            Category = category;
            Amount = amount ?? throw new ArgumentNullException(nameof(amount));
            CostCenterId = costCenterId;
            DepartmentId = departmentId;
            Description = description;
        }
    }
}