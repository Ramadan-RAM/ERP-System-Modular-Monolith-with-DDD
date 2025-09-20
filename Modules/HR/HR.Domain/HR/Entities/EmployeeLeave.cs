

namespace HR.Domain.HR.Entities
{
    public class EmployeeLeave
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public LeaveType LeaveType { get; set; } // Annual, Sick, Maternity, etc.
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
    }

    public enum LeaveType
    {
        Annual,
        Sick,
        Maternity,
        ReligiousHolidayMuslim,
        ReligiousHolidayChristian,
        Other
    }
}
