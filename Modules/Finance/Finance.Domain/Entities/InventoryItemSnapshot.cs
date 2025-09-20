// Finance.Domain/Entities/InventoryItemSnapshot.cs
using ERPSys.SharedKernel.Domain;
using Finance.Domain.Common.Enums;
using Finance.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Domain.Entities
{
    /// <summary>
    /// A snapshot of an inventory item's status for profit/loss forecasting (updated periodically via integration).
    /// </summary>
    public class InventoryItemSnapshot : BaseEntity<int>, IAggregateRoot
    {
        public Guid ProductId { get; private set; } // From Inventory
        public string SKU { get; private set; } = default!;
        public string ProductName { get; private set; } = default!;
        public InventoryItemType ItemType { get; private set; }
        public decimal QuantityAvailable { get; private set; }

        public Money AvgPurchaseCost { get; private set; } = default!;
        public Money AvgExpectedSalePrice { get; private set; } = default!;

        [NotMapped]
        public ExpiryInfo? Expiry { get; private set; }

        // Predictive analysis output 
        public Money ExpectedGrossProfit { get; private set; } = default!;
        public Money ExpectedLoss { get; private set; } = default!;
        public ProfitLossStatus ForecastStatus { get; private set; } = ProfitLossStatus.Neutral;
        public DateTime AsOf { get; private set; }
        public decimal AvgSellingPrice { get; set; }


        private InventoryItemSnapshot() { }

        public InventoryItemSnapshot(Guid productId,
        string sku,
        string productName,
        InventoryItemType type,
        decimal quantity,
        Money avgPurchaseCost,
        Money avgExpectedSalePrice,
        ExpiryInfo? expiry,
        DateTime asOf)
        {
            if (quantity < 0) throw new ArgumentException("Quantity cannot be negative");
            if (avgPurchaseCost.CurrencyCode != avgExpectedSalePrice.CurrencyCode)
                throw new InvalidOperationException("Currency mismatch.");

            ProductId = productId;
            SKU = sku.Trim();
            ProductName = productName.Trim();
            ItemType = type;
            QuantityAvailable = quantity;
            AvgPurchaseCost = avgPurchaseCost;
            AvgExpectedSalePrice = avgExpectedSalePrice;
            Expiry = expiry;
            AsOf = asOf.Date;

            Recalculate();
        }

        public void Recalculate()
        {
            var unitMargin = AvgExpectedSalePrice.Amount - AvgPurchaseCost.Amount;
            var gross = Math.Max(0m, unitMargin * QuantityAvailable);
            var loss = 0m;

            // Logic to reduce profitability or record potential loss depending on proximity to expiration
            if (Expiry?.IsPerishable == true)
            {
                var days = Expiry.DaysToExpire(AsOf);
                if (days.HasValue)
                {
                    if (days.Value < 0)
                    {
                        // Expired => Complete loss of purchase value
                        loss = AvgPurchaseCost.Amount * QuantityAvailable;
                        gross = 0m;
                        ForecastStatus = ProfitLossStatus.ExpectedLoss;
                    }
                    else if (days.Value <= Expiry.NearExpiryThresholdDays)
                    {
                        // Gradually deduct profitability as expiration approaches (e.g., 2% for each day remaining until the limit)
                        var factor = Math.Clamp(1m - (0.02m * (Expiry.NearExpiryThresholdDays - days.Value)), 0m, 1m);
                        gross = gross * factor;

                        // If the expected profitability falls below zero, consider the difference an expected loss.
                        if (gross <= 0m)
                        {
                            loss = Math.Abs(gross);
                            gross = 0m;
                            ForecastStatus = ProfitLossStatus.NearExpiryRisk;
                        }
                        else
                        {
                            ForecastStatus = ProfitLossStatus.NearExpiryRisk;
                        }
                    }
                    else
                    {
                        ForecastStatus = unitMargin > 0 ? ProfitLossStatus.ExpectedProfit : ProfitLossStatus.Neutral;
                    }
                }
            }
            else
            {
                ForecastStatus = unitMargin > 0 ? ProfitLossStatus.ExpectedProfit : ProfitLossStatus.Neutral;
            }

            ExpectedGrossProfit = new Money(gross, AvgPurchaseCost.CurrencyCode);
            ExpectedLoss = new Money(loss, AvgPurchaseCost.CurrencyCode);
        }
    }
}