using HR.Domain.HR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.HR.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FirstName)
                   .HasMaxLength(100)
                   .IsRequired();


            builder.Property(e => e.MiddelName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(e => e.LastName)
                   .HasMaxLength(100)
                   .IsRequired();

            // Relationship with the job title
            builder.HasOne(e => e.JobTitle)
            .WithMany()
            .HasForeignKey(e => e.JobTitleId)
            .OnDelete(DeleteBehavior.Restrict);

            // Relationship between manager and subordinates
            builder.HasOne(e => e.Manager)
            .WithMany(m => m.Subordinates)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

            // ✅ Global Query Filter for Soft Delete
            builder.HasQueryFilter(e => !e.IsDeleted);



        }
    }
}
