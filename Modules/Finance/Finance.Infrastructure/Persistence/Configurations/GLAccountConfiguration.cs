using Finance.Domain.Entities;
using Finance.Domain.Integrations.Links;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Persistence.Configurations
{
    public class GLAccountConfiguration : IEntityTypeConfiguration<GLAccount>
    {
        public void Configure(EntityTypeBuilder<GLAccount> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.BalanceSide).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();

            builder.HasOne(x => x.ParentAccount)
                   .WithMany()
                   .HasForeignKey(x => x.ParentAccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(x => x.Code, cb =>
            {
                cb.Property(c => c.Value)
                  .HasColumnName("AccountCode")
                  .IsRequired()
                  .HasMaxLength(20);
            });

            // Link accounts to cost centers (via DepartmentCostLink)
            builder.HasMany<DepartmentCostLink>()
                   .WithOne()
                   .HasForeignKey(x => x.GLAccountId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}