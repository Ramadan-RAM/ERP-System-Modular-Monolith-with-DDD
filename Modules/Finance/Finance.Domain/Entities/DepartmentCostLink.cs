using ERPSys.SharedKernel.Domain;
using Finance.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Domain.Entities
{
    /// <summary>
    /// Link a department in HR to a cost center and a financial account (GL Account).
    /// Objective: When an employee registers in HR, Finance knows how to deposit their expenses into CostCenter + GL Account.
    /// </summary>
    public class DepartmentCostLink : BaseEntity<int>, IAggregateRoot
    {
        public int Id { get; private set; }

        // Data coming from HR
        public int DepartmentId { get; private set; }
        public string DepartmentName { get; private set; } = string.Empty;
        public string JobTitle { get; private set; } = string.Empty;
        public string EmployeeCode { get; private set; } = string.Empty;
        public string EmployeeName { get; private set; } = string.Empty;
        public decimal NetSalary { get; private set; }

        // Link with Finance 
        public int CostCenterId { get; private set; }
        public CostCenter CostCenter { get; private set; } = null!;

        public int GLAccountId { get; private set; }
        [NotMapped]
        public GLAccount? GLAccount { get; private set; } = null!;

        protected DepartmentCostLink() { }

        public DepartmentCostLink(
        int departmentId,
        string departmentName,
        string jobTitle,
        string employeeCode,
        string employeeName,
        decimal netSalary,
        int costCenterId,
        int glAccountId)
        {
            DepartmentId = departmentId;
            DepartmentName = departmentName;
            JobTitle = jobTitle;
            EmployeeCode = employeeCode;
            EmployeeName = employeeName;
            NetSalary = netSalary;
            CostCenterId = costCenterId;
            GLAccountId = glAccountId;
        }

        public void UpdateDepartmentCostLink(
        int departmentId,
        string departmentName,
        string jobTitle,
        string employeeCode,
        string employeeName,
        decimal netSalary)
        {
            DepartmentId = departmentId;
            DepartmentName = departmentName;
            JobTitle = jobTitle;
            EmployeeCode = employeeCode;
            EmployeeName = employeeName;
            NetSalary = netSalary;
            UpdatedOn = DateTime.UtcNow;
        }

    }
}