
using ERPSys.SharedKernel.Domain;

namespace Finance.Domain.ValueObjects
{
    // Time period (for reports/budgets/fiscal year accounts)
    public sealed class Period : ValueObject
    {
        public DateTime Start { get; }
        public DateTime End { get; }
        public int Year { get; set; }
        public int Month { get; set; }

        private Period() { } // EF
        public Period(DateTime start, DateTime end)
        {
            if (end < start) throw new ArgumentException("End < Start");
            Start = start.Date;
            End = end.Date;
        }

        public bool Contains(DateTime date) => date.Date >= Start && date.Date <= End;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }

        public override string ToString() => $"{Start:yyyy-MM-dd}..{End:yyyy-MM-dd}";
    }
}
