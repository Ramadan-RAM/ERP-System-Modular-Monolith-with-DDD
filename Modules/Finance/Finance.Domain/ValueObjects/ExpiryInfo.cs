// Finance.Domain/Common/ValueObjects/ExpiryInfo.cs
using ERPSys.SharedKernel.Domain;

namespace Finance.Domain.ValueObjects
{
    // Product expiry information to estimate risk and profit loss as expiration approaches

    public sealed class ExpiryInfo : ValueObject
    {

        public DateTime? ExpiryDate { get; }

        public int NearExpiryThresholdDays { get; } // How many days before expiration are considered risky?

        public bool? IsPerishable => ExpiryDate.HasValue;

        public int WarningDays { get; set; }

        private ExpiryInfo() { } // EF 
        public ExpiryInfo(DateTime? expiryDate, bool v, int nearExpiryThresholdDays = 30)
        {
            if (nearExpiryThresholdDays < 0) throw new ArgumentException("Threshold must be >= 0");
            ExpiryDate = expiryDate?.Date;
            nearExpiryThresholdDays = nearExpiryThresholdDays;
        }

        public int? DaysToExpire(DateTime asOf)
        {
            if (!ExpiryDate.HasValue) return null;
            return (ExpiryDate.Value - asOf.Date).Days;
        }

        public bool IsNearExpiry(DateTime asOf)
        {
            var days = DaysToExpire(asOf);
            return days.HasValue && days.Value >= 0 && days.Value <= NearExpiryThresholdDays;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ExpiryDate;
            yield return NearExpiryThresholdDays;
        }
    }
}