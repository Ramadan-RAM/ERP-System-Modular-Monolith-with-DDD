using HR.Domain.HR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.HR.Configurations
{
    public class PayslipConfiguration : IEntityTypeConfiguration<Payslip>
    {
        public void Configure(EntityTypeBuilder<Payslip> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.PayDate)
                   .IsRequired();

            builder.Property(p => p.BasicSalary)
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Allowances)
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Deductions)
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Notes)
                   .HasMaxLength(500);

            builder.HasOne(p => p.Employee)
                   .WithMany()
                   .HasForeignKey(p => p.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(l => !l.IsDeleted); // ✅ Soft Delete filter
        }
    }
}
