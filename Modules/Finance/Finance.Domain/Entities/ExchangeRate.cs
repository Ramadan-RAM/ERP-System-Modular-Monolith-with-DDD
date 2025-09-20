// Finance.Domain/Entities/ExchangeRate.cs
using ERPSys.SharedKernel.Domain;

namespace Finance.Domain.Entities
{
    // Exchange rate of two currencies to evaluate restrictions/reports
    public class ExchangeRate : BaseEntity<int>, IAggregateRoot
    {
        public string FromCurrency { get; private set; } = default!;
        public string ToCurrency { get; private set; } = default!;
        public decimal Rate { get; private set; } // 1 From = Rate To
        public DateTime RateDate { get; private set; }

        private ExchangeRate() { }
        public ExchangeRate(string from, string to, decimal rate, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Currency codes required.");
            if (rate <= 0) throw new ArgumentException("Rate must be > 0");

            FromCurrency = from.ToUpperInvariant();
            ToCurrency = to.ToUpperInvariant();
            Rate = decimal.Round(rate, 6);
            RateDate = date.Date;
        }
    }
}
