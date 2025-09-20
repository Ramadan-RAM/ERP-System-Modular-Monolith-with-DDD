// Finance.Domain/Common/ValueObjects/Money.cs
using ERPSys.SharedKernel.Domain;

namespace Finance.Domain.ValueObjects
{
    /// <summary>
    /// VO for monetary value + currency. Commitment not to mix currencies in calculations.
    /// </summary>
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; }
        public string CurrencyCode { get; } // Example: "USD", "EGP", "SAR"
        public decimal Value { get; set; }

        private Money() { } // EF
        public Money(decimal amount, string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code is required", nameof(currencyCode));
            Amount = decimal.Round(amount, 2);
            CurrencyCode = currencyCode.ToUpperInvariant();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return CurrencyCode;
        }

        public Money Add(Money other)
        {
            EnsureSameCurrency(other);
            return new Money(Amount + other.Amount, CurrencyCode);
        }

        public Money Subtract(Money other)
        {
            EnsureSameCurrency(other);
            return new Money(Amount - other.Amount, CurrencyCode);
        }

        public Money Multiply(decimal factor) => new Money(Amount * factor, CurrencyCode);

        private void EnsureSameCurrency(Money other)
        {
            if (CurrencyCode != other.CurrencyCode)
                throw new InvalidOperationException("Currency mismatch.");
        }

        public override string ToString() => $"{Amount:0.00} {CurrencyCode}";
    }
}