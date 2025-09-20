

namespace Finance.Application.DTOs
{
    public class InventoryForecastDto
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal QuantityAvailable { get; set; }
        public decimal AvgPurchaseCost { get; set; }
        public decimal AvgExpectedSalePrice { get; set; }
        public decimal ExpectedGrossProfit { get; set; }
        public decimal ExpectedLoss { get; set; }
        public string ForecastStatus { get; set; } = string.Empty;
    }
}
