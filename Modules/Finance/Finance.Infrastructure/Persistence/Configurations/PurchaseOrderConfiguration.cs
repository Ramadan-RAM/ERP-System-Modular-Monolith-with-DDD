using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Persistence.Configurations
{
    public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.Number, num =>
            {
                num.Property(n => n.Value)
                   .HasColumnName("OrderNumber")
                   .HasMaxLength(20);
            });

            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);

            builder.OwnsOne(x => x.Total, tot =>
            {
                tot.Property(t => t.Amount).HasColumnName("TotalAmount");
                tot.Property(t => t.CurrencyCode).HasColumnName("TotalCurrency").HasMaxLength(3);
            });

            builder.HasMany(x => x.Items)
                   .WithOne(i => i.PurchaseOrder)
                   .HasForeignKey(i => i.PurchaseOrderId);
        }
    }
}
