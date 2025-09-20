
using HR.Domain.HR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.HR.Configurations
{
    public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
    {
        public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Date)
                .IsRequired();

            builder.HasOne(a => a.Employee)
                .WithMany() // It can be modified later if you add ICollection<AttendanceRecord> for the employee
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(a => a.DeviceId)
                .HasMaxLength(50);

            builder.Property(a => a.Source)
                .HasMaxLength(50);
        }
    }
}
