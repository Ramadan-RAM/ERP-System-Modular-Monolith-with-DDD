

namespace Finance.Application.DTOs
{
    public class JournalEntryDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<JournalLineDto> Lines { get; set; } = new();
    }
}
