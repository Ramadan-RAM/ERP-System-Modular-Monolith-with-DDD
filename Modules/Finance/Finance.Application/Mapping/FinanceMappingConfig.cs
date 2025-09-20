using Finance.Application.DTOs;
using Finance.Domain.Entities;
using Finance.Domain.ValueObjects;
using Mapster;

namespace Finance.Application.Mapping
{
    public static class FinanceMappingConfig
    {
        public static void RegisterMappings()
        {
            // 🌐 ValueObjects
            TypeAdapterConfig<Money, MoneyDto>
                .NewConfig()
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.CurrencyCode, src => src.CurrencyCode)
                .TwoWays();

            TypeAdapterConfig<ExpiryInfo, ExpiryInfoDto>
                .NewConfig()
                .Map(dest => dest.ExpiryDate, src => src.ExpiryDate)
                .Map(dest => dest.WarningDays, src => src.WarningDays)
                .TwoWays();

            // 📒 GL & Journal
            TypeAdapterConfig<GLAccount, GLAccountDto>.NewConfig().TwoWays();
            TypeAdapterConfig<JournalEntry, JournalEntryDto>.NewConfig().TwoWays();
            TypeAdapterConfig<JournalLine, JournalLineDto>.NewConfig().TwoWays();

            // 💱 Currency & Rates
            TypeAdapterConfig<Currency, CurrencyDto>.NewConfig().TwoWays();
            TypeAdapterConfig<ExchangeRate, ExchangeRateDto>.NewConfig().TwoWays();

            // 💸 Expenses
            TypeAdapterConfig<GeneralExpense, GeneralExpenseDto>.NewConfig().TwoWays();

            // 📦 Purchase Orders
            TypeAdapterConfig<PurchaseOrder, PurchaseOrderDto>.NewConfig().TwoWays();
            TypeAdapterConfig<PurchaseItem, PurchaseItemDto>.NewConfig().TwoWays();

            // 🏭 Inventory
            TypeAdapterConfig<InventoryItemSnapshot, InventorySnapshotDto>
                .NewConfig()
                .Map(dest => dest.AvgPurchaseCost, src => src.AvgPurchaseCost.Adapt<MoneyDto>())
                .Map(dest => dest.AvgSellingPrice, src => src.AvgSellingPrice.Adapt<MoneyDto>())
                .Map(dest => dest.Expiry, src => src.Expiry != null ? src.Expiry.Adapt<ExpiryInfoDto>() : null)
                .TwoWays();

            TypeAdapterConfig<InventoryItemSnapshot, InventoryForecastDto>.NewConfig().TwoWays();

            // 📊 Profit/Loss Forecast
            TypeAdapterConfig<ProfitLossForecast, ProfitLossForecastDto>
                .NewConfig()
                .Map(dest => dest.TotalExpectedProfit, src => src.TotalExpectedProfit.Adapt<MoneyDto>())
                .Map(dest => dest.TotalExpectedLoss, src => src.TotalExpectedLoss.Adapt<MoneyDto>())
                .TwoWays();
        }
    }
}
