using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Persistence.Configurations
{
    public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FromCurrency).HasMaxLength(3).IsRequired();
            builder.Property(x => x.ToCurrency).HasMaxLength(3).IsRequired();

            builder.Property(x => x.Rate).HasColumnType("decimal(18,6)");
        }
    }
}
