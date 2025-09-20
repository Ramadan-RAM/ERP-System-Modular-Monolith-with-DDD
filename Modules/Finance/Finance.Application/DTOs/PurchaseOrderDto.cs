

namespace Finance.Application.DTOs
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public Guid SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public List<PurchaseItemDto> Items { get; set; } = new();
    }

}
