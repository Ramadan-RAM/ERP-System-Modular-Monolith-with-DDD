using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Persistence.Configurations
{
    public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
    {
        public void Configure(EntityTypeBuilder<JournalEntry> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.EntryDate)
                   .IsRequired();

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.OwnsOne(x => x.Number, num =>
            {
                num.Property(n => n.Value)
                   .HasColumnName("JournalNumber")
                   .IsRequired()
                   .HasMaxLength(20);
            });

            builder.HasMany(x => x.Lines)
                   .WithOne(l => l.JournalEntry)
                   .HasForeignKey(l => l.JournalEntryId);
        }
    }
}
