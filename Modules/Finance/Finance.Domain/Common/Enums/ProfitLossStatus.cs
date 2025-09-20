// Finance.Domain/Common/Enums/ProfitLossStatus.cs
namespace Finance.Domain.Common.Enums
{
    // Financial forecast status of an item/group: Profit/Loss/Risk near expiration
    public enum ProfitLossStatus
    {
        Neutral = 0,
        ExpectedProfit = 1,
        ExpectedLoss = 2,
        NearExpiryRisk = 3
    }
}
