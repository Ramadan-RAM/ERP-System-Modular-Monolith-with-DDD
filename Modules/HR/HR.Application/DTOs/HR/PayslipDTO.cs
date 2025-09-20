namespace HR.Application.DTOs.HR
{
    public class PayslipDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateIssued { get; set; }
        public string? Notes { get; set; }

        public string? EmployeeName { get; set; } // For view only
    }

}
