// Finance.Domain/Common/Enums/PurchaseStatus.cs
namespace Finance.Domain.Common.Enums
{
    // Purchase order status
    public enum PurchaseStatus
    {
        Draft = 0,
        Submitted = 1,
        Approved = 2,
        PartiallyReceived = 3,
        FullyReceived = 4,
        Closed = 5,
        Cancelled = 6
    }
}
