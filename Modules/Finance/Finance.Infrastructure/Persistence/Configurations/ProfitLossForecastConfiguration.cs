// Finance.Infrastructure/Persistence/Configurations/ProfitLossForecastConfiguration.cs
using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Persistence.Configurations
{
    public class ProfitLossForecastConfiguration : IEntityTypeConfiguration<ProfitLossForecast>
    {
        public void Configure(EntityTypeBuilder<ProfitLossForecast> builder)
        {
            builder.HasKey(x => x.Id);

            // Period Value Object
            builder.OwnsOne(x => x.Period, period =>
            {
                period.Property(p => p.Start)
                      .HasColumnName("PeriodStartDate")
                      .IsRequired();

                period.Property(p => p.Start)
                      .HasColumnName("PeriodEndDate")
                      .IsRequired();
            });

            // Money: TotalExpectedProfit
            builder.OwnsOne(x => x.TotalExpectedProfit, profit =>
            {
                profit.Property(p => p.Amount)
                      .HasColumnName("TotalProfitAmount");

                profit.Property(p => p.CurrencyCode)
                      .HasColumnName("TotalProfitCurrency")
                      .HasMaxLength(3);
            });

            // Money: TotalExpectedLoss
            builder.OwnsOne(x => x.TotalExpectedLoss, loss =>
            {
                loss.Property(l => l.Amount)
                      .HasColumnName("TotalLossAmount");

                loss.Property(l => l.CurrencyCode)
                      .HasColumnName("TotalLossCurrency")
                      .HasMaxLength(3);
            });

            // Enum: ProfitLossStatus
            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(x => x.Notes)
                   .HasMaxLength(500);
        }
    }
}
