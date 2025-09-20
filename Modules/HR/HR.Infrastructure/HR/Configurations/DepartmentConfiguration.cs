
using HR.Domain.HR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.HR.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).HasMaxLength(100).IsRequired();


            builder.HasOne(d => d.Manager)
            .WithMany() // Don't bind the reverse now
            .HasForeignKey(d => d.ManagerId)
            .OnDelete(DeleteBehavior.Restrict); // So that the delete cycle doesn't occur


            builder.HasQueryFilter(d => !d.IsDeleted); // ✅ Soft Delete filter
        }
    }
}