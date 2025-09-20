using HR.Application.InterfacRepository;
using HR.Domain.HR.Entities;
using HR.Infrastructure.HR.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Infrastructure.HR.Services.ServicesRepository
{
    public class PayslipRepository : IPayslipRepository
    {
        private readonly HRDbContext _context;

        public PayslipRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task<List<Payslip>> GetAllAsync()
        {
            return await _context.Payslips.Include(x => x.Employee).ToListAsync();
        }

        public async Task<Payslip?> GetByIdAsync(int id)
        {
            return await _context.Payslips.Include(x => x.Employee).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Payslip> AddAsync(Payslip payslip)
        {
            _context.Payslips.Add(payslip);
            await _context.SaveChangesAsync();
            return payslip;
        }

        public async Task<bool> UpdateAsync(Payslip payslip)
        {
            _context.Payslips.Update(payslip);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var payslip = await _context.Payslips.FindAsync(id);
            if (payslip == null) return false;

            _context.Payslips.Remove(payslip);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(int employeeId, int year, int month)
        {
            return await _context.Payslips.AnyAsync(p =>
                p.EmployeeId == employeeId &&
                p.PayDate.Year == year &&
                p.PayDate.Month == month);
        }

    }
}
