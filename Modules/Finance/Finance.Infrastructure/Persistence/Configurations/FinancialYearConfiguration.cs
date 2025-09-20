// Finance.Infrastructure/Configurations/FinancialYearConfiguration.cs
using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Configurations
{
    public class FinancialYearConfiguration : IEntityTypeConfiguration<FinancialYear>
    {
        public void Configure(EntityTypeBuilder<FinancialYear> builder)
        {
            // Table name
            builder.ToTable("FinancialYears");

            // Primary Key
            builder.HasKey(f => f.Id);

            // Properties
            builder.Property(f => f.Year)
                   .IsRequired();

            builder.Property(f => f.IsClosed)
                   .HasDefaultValue(false);

            // Value Object: Period
            builder.OwnsOne(f => f.Period, period =>
            {
                period.Property(p => p.Start)
                      .HasColumnName("StartDate")
                      .IsRequired();

                period.Property(p => p.End)
                      .HasColumnName("EndDate")
                      .IsRequired();
            });

            // Index for Year (unique per year)
            builder.HasIndex(f => f.Year)
                   .IsUnique();
        }
    }
}
