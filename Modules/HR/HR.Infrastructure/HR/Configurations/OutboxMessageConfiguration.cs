using HR.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Infrastructure.HR.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Payload)
                .IsRequired();

            builder.Property(x => x.OccurredOn)
                .IsRequired();

            builder.Property(x => x.Sent)
                .HasDefaultValue(false);

            builder.Property(x => x.SentOn)
                .IsRequired(false);
        }
    }
}
