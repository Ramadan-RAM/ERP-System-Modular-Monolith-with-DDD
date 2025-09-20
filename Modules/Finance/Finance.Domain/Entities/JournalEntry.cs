// Finance.Domain/Entities/JournalEntry.cs
using ERPSys.SharedKernel.Domain;
using Finance.Domain.Common.Enums;
using Finance.Domain.ValueObjects;

namespace Finance.Domain.Entities
{
    /// <summary>
    /// Journal entry (entry header). Contains multiple JournalLine items.
    /// </summary>
    public class JournalEntry : BaseEntity<int>, IAggregateRoot
    {
        public DocumentNumber Number { get;  set; } = default!;
        public DateTime EntryDate { get;  set; }
        public JournalStatus Status { get;  set; } = JournalStatus.Draft;

        private readonly List<JournalLine> _lines = new();
        public IReadOnlyCollection<JournalLine> Lines => _lines.AsReadOnly();

        private JournalEntry() { }

        public JournalEntry(DocumentNumber number, DateTime entryDate)
        {
            Number = number ?? throw new ArgumentNullException(nameof(number));
            EntryDate = entryDate.Date;
        }

        public void AddLine(JournalLine line) => _lines.Add(line ?? throw new ArgumentNullException(nameof(line)));

        public bool IsBalanced()
        {
            var currency = _lines.FirstOrDefault()?.CurrencyCode ?? "XXX";
            var debit = _lines.Where(l => l.Debit != null).Aggregate(0m, (s, l) => s + l.Debit!.Amount);
            var credit = _lines.Where(l => l.Credit != null).Aggregate(0m, (s, l) => s + l.Credit!.Amount);
            return decimal.Round(debit - credit, 2) == 0m;
        }

        public void Post()
        {
            if (!IsBalanced())
                throw new InvalidOperationException("Journal not balanced.");
            Status = JournalStatus.Posted;
            // Emit Domain Event if needed (e.g., JournalPostedDomainEvent)
        }


        // في JournalEntry.cs (Entity)
        public void Approve()
        {
            if (Status == JournalStatus.Approved)
                throw new InvalidOperationException("Already approved.");

            Status = JournalStatus.Approved;
        }


    }
}
