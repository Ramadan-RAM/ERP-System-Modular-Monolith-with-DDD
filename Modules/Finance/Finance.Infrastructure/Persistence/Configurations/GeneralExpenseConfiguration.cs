using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Persistence.Configurations
{
    public class GeneralExpenseConfiguration : IEntityTypeConfiguration<GeneralExpense>
    {
        public void Configure(EntityTypeBuilder<GeneralExpense> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ExpenseDate).IsRequired();
            builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(100);

            builder.OwnsOne(x => x.Amount, amt =>
            {
                amt.Property(a => a.Amount).HasColumnName("ExpenseAmount");
                amt.Property(a => a.CurrencyCode).HasColumnName("ExpenseCurrency").HasMaxLength(3);
            });
        }
    }
}
