using ERPSys.SharedKernel.Domain;

namespace HR.Domain.HR.Entities
{
    public class Payslip : BaseEntity<int>
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public DateTime PayDate { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary => BasicSalary + Allowances - Deductions;

        public string Notes { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;
    }

}
