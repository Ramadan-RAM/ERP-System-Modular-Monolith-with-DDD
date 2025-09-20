using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Persistence.Configurations
{
    public class JournalLineConfiguration : IEntityTypeConfiguration<JournalLine>
    {
        public void Configure(EntityTypeBuilder<JournalLine> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description)
                   .HasMaxLength(500);

            builder.OwnsOne(x => x.Debit, debit =>
            {
                debit.Property(d => d.Amount).HasColumnName("DebitAmount");
                debit.Property(d => d.CurrencyCode).HasColumnName("DebitCurrency").HasMaxLength(3);
            });

            builder.OwnsOne(x => x.Credit, credit =>
            {
                credit.Property(c => c.Amount).HasColumnName("CreditAmount");
                credit.Property(c => c.CurrencyCode).HasColumnName("CreditCurrency").HasMaxLength(3);
            });
        }
    }
}
