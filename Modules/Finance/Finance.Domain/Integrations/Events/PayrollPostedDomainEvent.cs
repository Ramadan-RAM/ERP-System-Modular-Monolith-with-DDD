// Finance.Domain/Integrations/Events/PayrollPostedDomainEvent.cs
using ERPSys.SharedKernel.Events;
using Finance.Domain.ValueObjects;

namespace Finance.Domain.Integrations.Events
{
    /// <summary>
    /// Triggered by HR when payroll is approved/posted, to create payroll entries.
    /// </summary>
    public sealed class PayrollPostedDomainEvent : DomainEvent
    {
        public Guid PayrollBatchId { get; }
        public Money TotalNetSalaries { get; }
        public DateTime PeriodEnd { get; }

        public PayrollPostedDomainEvent(Guid payrollBatchId, Money totalNetSalaries, DateTime periodEnd)
        {
            PayrollBatchId = payrollBatchId;
            TotalNetSalaries = totalNetSalaries;
            PeriodEnd = periodEnd.Date;
        }
    }
}
