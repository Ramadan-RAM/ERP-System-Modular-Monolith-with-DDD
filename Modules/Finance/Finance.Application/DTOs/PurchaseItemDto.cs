

namespace Finance.Application.DTOs
{
    public class PurchaseItemDto
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
    }
}
