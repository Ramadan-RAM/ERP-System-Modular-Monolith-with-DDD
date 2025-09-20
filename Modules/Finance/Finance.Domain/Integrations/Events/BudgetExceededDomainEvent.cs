// Finance.Domain/Integrations/Events/BudgetExceededDomainEvent.cs
using ERPSys.SharedKernel.Events;
using Finance.Domain.ValueObjects;

namespace Finance.Domain.Integrations.Events
{
    /// <summary>
    /// When a cost center exceeds its budget during a specific period.
    /// </summary>
    public sealed class BudgetExceededDomainEvent : DomainEvent
    {
        public int CostCenterId { get; }
        public Period Period { get; }
        public decimal BudgetAmount { get; }
        public decimal ActualAmount { get; }

        public BudgetExceededDomainEvent(int costCenterId, Period period, decimal budgetAmount, decimal actualAmount)
        {
            CostCenterId = costCenterId;
            Period = period;
            BudgetAmount = budgetAmount;
            ActualAmount = actualAmount;
        }
    }
}
