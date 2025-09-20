using Finance.Application.Interfaces;
using Finance.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Integration.Services
{
    public class AccountMappingService : IAccountMappingService
    {
        private readonly FinanceDbContext _dbContext;

        public AccountMappingService(FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Answers the expense account for the Department or Employee 
        public async Task<int> GetSalaryExpenseAccountId(int departmentId)
        {
            var link = await _dbContext.DepartmentCostLinks
            .FirstOrDefaultAsync(l => l.DepartmentId == departmentId);

            if (link == null)
                throw new InvalidOperationException($"❌ No salary expense account found for department {departmentId}");

            return link.GLAccountId; // Returns the GLAccountId for the employee or department
        }

        // Gets the payment account (Cash/Bank) from the same link or default
        public async Task<int> GetCashAccountId(int departmentId, string employeeCode)
        {
            var link = await _dbContext.DepartmentCostLinks
            .FirstOrDefaultAsync(l => l.DepartmentId == departmentId && l.EmployeeCode == employeeCode);

            if (link == null)
                throw new InvalidOperationException($"❌ No cash account mapping found for Employee {employeeCode} in Department {departmentId}");

            return link.GLAccountId; // The payable account associated with the employee
        }
    }
}