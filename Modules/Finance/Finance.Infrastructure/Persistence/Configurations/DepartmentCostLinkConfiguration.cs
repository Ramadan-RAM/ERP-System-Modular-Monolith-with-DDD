using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Persistence.Configurations
{
    public class DepartmentCostLinkConfiguration : IEntityTypeConfiguration<DepartmentCostLink>
    {
        public void Configure(EntityTypeBuilder<DepartmentCostLink> builder)
        {
            builder.HasKey(x => x.Id);

            // HR Props
            builder.Property(x => x.DepartmentId).IsRequired();
            builder.Property(x => x.DepartmentName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.JobTitle).HasMaxLength(100);
            builder.Property(x => x.EmployeeCode).HasMaxLength(50);
            builder.Property(x => x.EmployeeName).HasMaxLength(200);
            builder.Property(x => x.NetSalary).HasColumnType("decimal(18,2)");

            // Relations
            builder.HasOne(x => x.CostCenter)
                   .WithMany()
                   .HasForeignKey(x => x.CostCenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne(x => x.GLAccount)
            //       .WithMany()
            //       .HasForeignKey(x => x.GLAccountId)
            //       .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.DepartmentId, x.EmployeeCode })
                   .IsUnique(false);
        }
    }
}
