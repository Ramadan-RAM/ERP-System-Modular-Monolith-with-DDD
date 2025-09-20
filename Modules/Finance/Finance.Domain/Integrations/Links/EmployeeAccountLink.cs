using ERPSys.SharedKernel.Domain;


namespace Finance.Domain.Integrations.Links
{
    /// <summary>
    /// Link an employee (from HR) to a GL account to receive/post payroll entries.
    /// </summary>
    public class EmployeeAccountLink : BaseEntity<int>
    {
        public int EmployeeId { get; private set; }   // HR Employee
        public int GLAccountId { get; private set; }  // Linked GL Account

        private EmployeeAccountLink() { } // EF

        public EmployeeAccountLink(int employeeId, int glAccountId)
        {
            if (employeeId <= 0) throw new ArgumentException("Invalid EmployeeId");
            if (glAccountId <= 0) throw new ArgumentException("Invalid GLAccountId");

            EmployeeId = employeeId;
            GLAccountId = glAccountId;
        }
    }
}
