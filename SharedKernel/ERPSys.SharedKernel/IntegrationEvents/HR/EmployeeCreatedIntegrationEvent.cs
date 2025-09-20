using ERPSys.SharedKernel.Events;

namespace SharedKernel.IntegrationEvents.HR
{
    /// <summary>
    /// Event published when a new employee is created (Integration Event).
    /// Used by Finance to onboard employee accounts.
    /// </summary>
    // ERPSys.SharedKernel.IntegrationEvents.HR
    public class EmployeeCreatedIntegrationEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public int EmployeeId { get; }
        public int DepartmentId { get; }
        public string DepartmentName { get; }
        public string JobTitle { get; }
        public string EmployeeCode { get; }
        public string FullName { get; }
        public decimal NetSalary { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public EmployeeCreatedIntegrationEvent(
            int employeeId,
            int departmentId,
            string departmentName,
            string jobTitle,
            string employeeCode,
            string fullName,
            decimal netSalary)
        {
            EmployeeId = employeeId;
            DepartmentId = departmentId;   
            DepartmentName = departmentName;
            JobTitle = jobTitle;
            EmployeeCode = employeeCode;
            FullName = fullName;
            NetSalary = netSalary;
        }
    }

}
