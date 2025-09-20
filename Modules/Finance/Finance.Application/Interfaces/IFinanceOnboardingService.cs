using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.Interfaces
{
    public interface IFinanceOnboardingService
    {
        Task CreateEmployeeAccountsAsync(int employeeId, int departmentId, string? department, string? jobTitle, string employeeCode, string fullName, decimal netSalary);
        Task UpdateEmployeeAccountsAsync(int employeeId, int departmentId, string? department, string? jobTitle, string employeeCode, string fullName, decimal netSalary);
    }
}
