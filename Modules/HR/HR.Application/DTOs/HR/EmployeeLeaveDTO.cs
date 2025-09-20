namespace HR.Application.DTOs.HR
{
    public class EmployeeLeaveDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string LeaveType { get; set; } // Use string to store enum as readable
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
    }
}
