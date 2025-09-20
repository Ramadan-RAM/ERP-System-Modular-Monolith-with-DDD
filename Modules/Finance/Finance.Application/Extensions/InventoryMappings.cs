using Finance.Application.DTOs;
using Finance.Domain.Entities;

namespace Finance.Application.Extensions
{
    public static class InventoryMappings
    {
        public static InventorySnapshotDto ToDto(this InventoryItemSnapshot entity)
        {
            return new InventorySnapshotDto
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                SKU = entity.SKU,
                ProductName = entity.ProductName,
                ItemType = entity.ItemType.ToString(),
                Quantity = (int)entity.QuantityAvailable, 
                AvgPurchaseCost = new MoneyDto
                {
                    Amount = entity.AvgPurchaseCost.Amount,
                    CurrencyCode = entity.AvgPurchaseCost.CurrencyCode
                },
                AvgSellingPrice = new MoneyDto 
                {
                    Amount = entity.AvgExpectedSalePrice.Amount,
                    CurrencyCode = entity.AvgExpectedSalePrice.CurrencyCode
                },
                Expiry = entity.Expiry == null
                    ? null
                    : new ExpiryInfoDto
                    {
                        ExpiryDate = entity.Expiry.ExpiryDate
                    },
                AsOf = entity.AsOf,
                ExpectedGrossProfit = entity.ExpectedGrossProfit.Amount,
                ExpectedLoss = entity.ExpectedLoss.Amount,
                ForecastStatus = entity.ForecastStatus.ToString()
            };
        }
    }
}
