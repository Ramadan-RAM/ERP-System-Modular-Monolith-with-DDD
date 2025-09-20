namespace HR.Application.DTOs.HR
{
    public class EmployeeDTO
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string MiddelName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = "Male";

        public string Department { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;

        public decimal BasicSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }

        public string? PictureUrl { get; set; }
        public string? CVUrl { get; set; }

    }
}
