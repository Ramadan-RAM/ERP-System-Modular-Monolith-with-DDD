using HR.Domain.HR.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.InterfacRepository
{
    public interface IPayslipRepository
    {
        Task<List<Payslip>> GetAllAsync();
        Task<Payslip?> GetByIdAsync(int id);
        Task<Payslip> AddAsync(Payslip payslip);
        Task<bool> UpdateAsync(Payslip payslip);
        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsAsync(int employeeId, int year, int month);

    }
}
