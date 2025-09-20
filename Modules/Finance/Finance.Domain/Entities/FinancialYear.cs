// Finance.Domain/Entities/FinancialYear.cs
using ERPSys.SharedKernel.Domain;
using Finance.Domain.ValueObjects;

namespace Finance.Domain.Entities
{
    // Fiscal year (or larger fiscal period) to set the closing
    public class FinancialYear : BaseEntity<int>, IAggregateRoot
    {
        public int Year { get; private set; }
        public Period Period { get; private set; } = default!;
        public bool IsClosed { get; private set; }

        private FinancialYear() { }
        public FinancialYear(int year, Period period)
        {
            Year = year;
            Period = period ?? throw new ArgumentNullException(nameof(period));
            IsClosed = false;
        }

        public void Close() => IsClosed = true;
        public void Reopen() => IsClosed = false;
    }
}
