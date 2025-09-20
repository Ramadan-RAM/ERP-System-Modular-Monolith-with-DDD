

namespace Finance.Application.DTOs
{
    public class JournalLineDto
    {
        public int Id { get; set; }
        public int GLAccountId { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public int? CostCenterId { get; set; }
        public string? Description { get; set; }
        public string? ExternalReference { get; set; }
    }
}
