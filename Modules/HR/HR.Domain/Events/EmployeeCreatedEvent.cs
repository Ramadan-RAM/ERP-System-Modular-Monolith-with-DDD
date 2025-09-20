using ERPSys.SharedKernel.Events;
using HR.Domain.HR.Entities;

namespace HR.Domain.Events
{
    using ERPSys.SharedKernel.Events;

    namespace HR.Domain.Events
    {
        // HR.Domain.Events
        public class EmployeeCreatedEvent : IDomainEvent
        {
            public int EmployeeId { get; }
            public int DepartmentId { get; }   // ✅ جديد
            public string DepartmentName { get; }
            public string JobTitle { get; }
            public string EmployeeCode { get; }
            public string FullName { get; }
            public decimal NetSalary { get; }
            public DateTime OccurredOn { get; } = DateTime.UtcNow;

            public EmployeeCreatedEvent(
                int employeeId,
                int departmentId,
                string departmentName,
                string jobTitle,
                string employeeCode,
                string fullName,
                decimal netSalary)
            {
                EmployeeId = employeeId;
                DepartmentId = departmentId;   // ✅
                DepartmentName = departmentName;
                JobTitle = jobTitle;
                EmployeeCode = employeeCode;
                FullName = fullName;
                NetSalary = netSalary;
            }
        }

    }

}
