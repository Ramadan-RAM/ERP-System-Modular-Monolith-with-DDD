using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Persistence.Configurations
{
    public class ProcessedEventLogConfiguration : IEntityTypeConfiguration<ProcessedEventLog>
    {
        public void Configure(EntityTypeBuilder<ProcessedEventLog> builder)
        {
            // Determine the primary key
            builder.HasKey(x => x.EventId);

            builder.Property(x => x.HandlerName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.ProcessedOn)
                   .IsRequired();

            // اسم الجدول
            builder.ToTable("ProcessedEventLogs");
        }
    }
}
