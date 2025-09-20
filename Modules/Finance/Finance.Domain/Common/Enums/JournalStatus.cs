// Finance.Domain/Common/Enums/JournalStatus.cs
namespace Finance.Domain.Common.Enums
{
    // Registration status: Draft, Stage, Posted
    public enum JournalStatus
    {
        Draft = 0,
        PendingApproval = 1,
        Posted = 2,
        Rejected = 3,
        Approved = 4
    }
}
