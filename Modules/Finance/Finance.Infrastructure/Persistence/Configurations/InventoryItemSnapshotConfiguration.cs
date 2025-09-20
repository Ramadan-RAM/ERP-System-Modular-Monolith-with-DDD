using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Persistence.Configurations
{
    public class InventoryItemSnapshotConfiguration : IEntityTypeConfiguration<InventoryItemSnapshot>
    {
        public void Configure(EntityTypeBuilder<InventoryItemSnapshot> builder)
        {
            builder.HasKey(x => x.Id);

            // 📌 AvgPurchaseCost
            builder.OwnsOne(x => x.AvgPurchaseCost, cost =>
            {
                cost.Property(c => c.Amount).HasColumnName("AvgPurchaseAmount");
                cost.Property(c => c.CurrencyCode).HasColumnName("AvgPurchaseCurrency").HasMaxLength(3);
            });

            // 📌 AvgExpectedSalePrice
            builder.OwnsOne(x => x.AvgExpectedSalePrice, price =>
            {
                price.Property(p => p.Amount).HasColumnName("AvgSaleAmount");
                price.Property(p => p.CurrencyCode).HasColumnName("AvgSaleCurrency").HasMaxLength(3);
            });

            // 📌 ExpectedGrossProfit
            builder.OwnsOne(x => x.ExpectedGrossProfit, profit =>
            {
                profit.Property(p => p.Amount).HasColumnName("GrossProfitAmount");
                profit.Property(p => p.CurrencyCode).HasColumnName("GrossProfitCurrency").HasMaxLength(3);
            });

            // 📌 ExpectedLoss
            builder.OwnsOne(x => x.ExpectedLoss, loss =>
            {
                loss.Property(l => l.Amount).HasColumnName("LossAmount");
                loss.Property(l => l.CurrencyCode).HasColumnName("LossCurrency").HasMaxLength(3);
            });

            // 📌 ExpiryInfo
            builder.OwnsOne(x => x.Expiry, expiry =>
            {
                expiry.Property(e => e.ExpiryDate).HasColumnName("ExpiryDate");
                expiry.Property(e => e.NearExpiryThresholdDays).HasColumnName("NearExpiryThresholdDays");
                expiry.Property(e => e.WarningDays).HasColumnName("WarningDays");

                // Ignore the calculated property
                expiry.Ignore(e => e.IsPerishable);
            });
        }
    }
}
