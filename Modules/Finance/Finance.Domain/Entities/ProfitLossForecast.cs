// Finance.Domain/Entities/ProfitLossForecast.cs

using Finance.Domain.Common.Enums;
using ERPSys.SharedKernel.Domain;
using Finance.Domain.ValueObjects;

namespace Finance.Domain.Entities
{
    /// <summary>
    /// ملخص تنبؤي لفترة/مجموعة أصناف (ناتج تجميع Snapshots).
    /// Used for monthly/annual profit reporting and waste reduction.
    /// </summary>
    public class ProfitLossForecast : BaseEntity<int>, IAggregateRoot
    {
        public Period Period { get; private set; } = default!;
        public Money TotalExpectedProfit { get; private set; } = default!;
        public Money TotalExpectedLoss { get; private set; } = default!;
        public ProfitLossStatus Status { get; private set; } = ProfitLossStatus.Neutral;
        public string? Notes { get; private set; }

        private ProfitLossForecast() { }

        public ProfitLossForecast(Period period, Money profit, Money loss, string? notes = null)
        {
            if (profit.CurrencyCode != loss.CurrencyCode)
                throw new InvalidOperationException("Currency mismatch.");

            Period = period ?? throw new ArgumentNullException(nameof(period));
            TotalExpectedProfit = profit;
            TotalExpectedLoss = loss;
            Notes = notes;

            Status = loss.Amount > 0m
                ? ProfitLossStatus.ExpectedLoss
                : (profit.Amount > 0m ? ProfitLossStatus.ExpectedProfit : ProfitLossStatus.Neutral);
        }
    }
}
