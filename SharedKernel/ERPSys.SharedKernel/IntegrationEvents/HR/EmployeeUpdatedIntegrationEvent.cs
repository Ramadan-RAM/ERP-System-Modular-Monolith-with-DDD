using ERPSys.SharedKernel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSys.SharedKernel.IntegrationEvents.HR
{
    public class EmployeeUpdatedIntegrationEvent : IDomainEvent
    {
        public int EmployeeId { get; }
        public int DepartmentId { get; }
        public string DepartmentName { get; }
        public string JobTitle { get; }
        public string EmployeeCode { get; }
        public string FullName { get; }
        public decimal NetSalary { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public EmployeeUpdatedIntegrationEvent(
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
