using HR.Domain.HR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.HR.Configurations
{
    public class EmployeeLeaveConfiguration : IEntityTypeConfiguration<EmployeeLeave>
    {
        public void Configure(EntityTypeBuilder<EmployeeLeave> builder)
        {
            builder.ToTable("Leaves"); 

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Reason)
                   .HasMaxLength(250);

            builder.HasOne(x => x.Employee)
                   .WithMany(e => e.Leaves)
                   .HasForeignKey(x => x.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
