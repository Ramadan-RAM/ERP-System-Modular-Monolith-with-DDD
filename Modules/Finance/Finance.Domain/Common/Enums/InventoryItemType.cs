// Finance.Domain/Common/Enums/InventoryItemType.cs
namespace Finance.Domain.Common.Enums
{
    // Inventory item type (for profit/loss forecasting purposes, especially expirable)
    public enum InventoryItemType
    {
        General = 0,
        Food = 1,
        Medicine = 2,
        Cosmetic = 3,
        Chemical = 4,
        OtherPerishable = 5,
        FinishedGood = 6,
        RawMaterial = 7
    }
}
