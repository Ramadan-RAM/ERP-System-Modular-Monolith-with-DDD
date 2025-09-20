using ERPSys.SharedKernel.Persistence;
using Finance.Application.Interfaces;
using Finance.Domain.Common.Enums;
using Finance.Domain.Entities;
using Finance.Domain.ValueObjects;
using Logging.Application.Interfaces;

namespace Finance.Application.Services
{
    public class FinanceOnboardingService : IFinanceOnboardingService
    {
        private readonly IRepository<GLAccount, int> _glRepo;
        private readonly IRepository<CostCenter, int> _costCenterRepo;
        private readonly IRepository<DepartmentCostLink, int> _deptCostLinkRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILoggingService _logService;

        public FinanceOnboardingService(
            IRepository<GLAccount, int> glRepo,
            IRepository<CostCenter, int> costCenterRepo,
            IRepository<DepartmentCostLink, int> deptCostLinkRepo,
            IUnitOfWork uow,
            ILoggingService logService)
        {
            _glRepo = glRepo;
            _costCenterRepo = costCenterRepo;
            _deptCostLinkRepo = deptCostLinkRepo;
            _uow = uow;
            _logService = logService;
        }

        public async Task CreateEmployeeAccountsAsync(
            int employeeId,
            int departmentId,
            string? departmentName,
            string jobTitle,
            string employeeCode,
            string fullName,
            decimal netSalary)
        {
            try
            {
                // 1) Ensure CostCenter for department
                var existingCenter = (await _costCenterRepo.ListAllAsync())
                    .FirstOrDefault(c => c.DepartmentId == departmentId || c.Name == departmentName);

                if (existingCenter == null)
                {
                    var code = !string.IsNullOrWhiteSpace(departmentName) && departmentName.Length >= 3
                        ? departmentName.Substring(0, 3).ToUpper()
                        : "GEN";

                    existingCenter = new CostCenter(code, departmentName ?? "General", departmentId);
                    await _costCenterRepo.AddAsync(existingCenter);
                    await _uow.SaveChangesAsync();
                }

                // 2) Ensure GL Account for employee
                var accountCode = new AccountCode($"EMP-{employeeId}");
                var existingAccount = (await _glRepo.ListAllAsync())
                    .FirstOrDefault(a => a.Code.Value == accountCode.Value);

                if (existingAccount == null)
                {
                    existingAccount = new GLAccount(
                        accountCode,
                        $"Employee Payroll - {fullName}",
                        AccountType.Expense,
                        BalanceSide.Debit
                    );

                    await _glRepo.AddAsync(existingAccount);
                    await _uow.SaveChangesAsync();
                }

                // 3) Ensure DepartmentCostLink (with employee details)
                var existingLink = (await _deptCostLinkRepo.ListAllAsync())
                    .FirstOrDefault(l => l.EmployeeCode == employeeCode);

                if (existingLink == null)
                {
                    var deptLink = new DepartmentCostLink(
                        departmentId,
                        departmentName ?? "Unknown",
                        jobTitle,
                        employeeCode,
                        fullName,
                        netSalary,
                        existingCenter.Id,
                        existingAccount.Id
                    );

                    await _deptCostLinkRepo.AddAsync(deptLink);
                    await _uow.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Finance", nameof(FinanceOnboardingService), nameof(CreateEmployeeAccountsAsync),
                    ex, new { employeeId, departmentId, departmentName, jobTitle, employeeCode, fullName, netSalary });
                throw;
            }
        }

        public async Task UpdateEmployeeAccountsAsync(
        int employeeId,
        int departmentId,
        string departmentName,
        string jobTitle,
        string employeeCode,
        string fullName,
        decimal netSalary)
        {
            try
            {
                var existingLink = (await _deptCostLinkRepo.ListAllAsync())
                   .FirstOrDefault(l => l.EmployeeCode == employeeCode || l.Id == employeeId);


                if (existingLink != null)
                {
                    // ✅ Update data
                    existingLink.UpdateDepartmentCostLink(
                        departmentId,
                        departmentName,
                        jobTitle,
                        employeeCode,
                        fullName,
                        netSalary
                    );

                    await _deptCostLinkRepo.UpdateAsync(existingLink);
             
                    await _uow.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Finance", nameof(FinanceOnboardingService), nameof(UpdateEmployeeAccountsAsync),
                    ex, new { employeeId, departmentId, departmentName, jobTitle, employeeCode, fullName, netSalary });
                throw;
            }
        }

    }
}
