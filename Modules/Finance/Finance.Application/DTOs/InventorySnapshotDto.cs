namespace Finance.Application.DTOs
{
    public class InventorySnapshotDto
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public MoneyDto AvgPurchaseCost { get; set; } = null!;
        public MoneyDto AvgSellingPrice { get; set; } = null!;
        public ExpiryInfoDto? Expiry { get; set; }
        public DateTime AsOf { get; set; }
        public decimal ExpectedGrossProfit { get; set; }
        public decimal ExpectedLoss { get; set; }
        public string ForecastStatus { get; set; } = string.Empty;
    }
}
