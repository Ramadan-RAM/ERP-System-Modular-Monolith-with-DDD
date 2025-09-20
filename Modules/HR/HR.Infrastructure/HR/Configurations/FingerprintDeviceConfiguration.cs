using HR.Domain.HR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.HR.Configurations
{
    public class FingerprintDeviceConfiguration : IEntityTypeConfiguration<FingerprintDevice>
    {
        public void Configure(EntityTypeBuilder<FingerprintDevice> builder)
        {
            builder.ToTable("FingerprintDevices");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.IP)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(d => d.Type)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasOne(d => d.Branch)
                   .WithMany(b => b.Devices)
                   .HasForeignKey(d => d.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
