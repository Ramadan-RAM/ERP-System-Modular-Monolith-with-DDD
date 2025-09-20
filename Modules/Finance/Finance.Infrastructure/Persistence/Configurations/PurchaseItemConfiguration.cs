using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Persistence.Configurations
{
    public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.UnitPrice, price =>
            {
                price.Property(p => p.Amount).HasColumnName("UnitPrice");
                price.Property(p => p.CurrencyCode).HasColumnName("PriceCurrency").HasMaxLength(3);
            });

            builder.OwnsOne(x => x.ExpectedSalePrice, price =>
            {
                price.Property(p => p.Amount).HasColumnName("SalePrice");
                price.Property(p => p.CurrencyCode).HasColumnName("SaleCurrency").HasMaxLength(3);
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
