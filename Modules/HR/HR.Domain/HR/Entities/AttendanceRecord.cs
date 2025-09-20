using ERPSys.SharedKernel.Domain;
using HR.Domain.Common;

namespace HR.Domain.HR.Entities
{
    public class AttendanceRecord : BaseEntity<int>
    {
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public string? DeviceId { get; set; }
        public string? Source { get; set; }

        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
    }
}
