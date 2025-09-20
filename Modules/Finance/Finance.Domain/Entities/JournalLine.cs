// Finance.Domain/Entities/JournalLine.cs
using ERPSys.SharedKernel.Domain;
using Finance.Domain.ValueObjects;

namespace Finance.Domain.Entities
{
    /// <summary>
    /// Constraint item. May be linked to a cost center or an external reference (invoice/payroll).
    /// </summary>
    public class JournalLine : BaseEntity<int>, IAggregateRoot
    {
        public int JournalEntryId { get;  set; }
        public JournalEntry JournalEntry { get; set; } = default!;

        public int GLAccountId { get;  set; }
        public GLAccount GLAccount { get;  set; } = default!;

        public Money? Debit { get;  set; }
        public Money? Credit { get;  set; }
        public string CurrencyCode => Debit?.CurrencyCode ?? Credit?.CurrencyCode ?? "XXX";

        public int? CostCenterId { get; set; }
        public CostCenter? CostCenter { get; set; }

        public string? Description { get; set; }
        public string? ExternalReference { get; set; } // InvoiceId, PayrollId, POId...

        // ✅ EF Core needs a parameterless ctor
        private JournalLine() { }

        // ✅ Domain constructor
        public JournalLine(int glAccountId, Money? debit, Money? credit, int? costCenterId = null, string? description = null, string? externalRef = null)
        {
            if (debit == null && credit == null)
                throw new ArgumentException("Either debit or credit must be provided.");
            if (debit != null && credit != null)
                throw new ArgumentException("Cannot set both debit and credit.");

            GLAccountId = glAccountId;
            Debit = debit;
            Credit = credit;
            CostCenterId = costCenterId;
            Description = description;
            ExternalReference = externalRef;
        }
    }

}
