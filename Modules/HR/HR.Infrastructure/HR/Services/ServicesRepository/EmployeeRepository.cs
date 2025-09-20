using HR.Application.Interfaces.InterfacRepository;
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
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly HRDbContext _context;

        public EmployeeRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees.Include(e => e.JobTitle).ThenInclude(j => j.Department)
                                           .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees.Where(e => !e.IsDeleted).ToListAsync();
        }

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp != null)
            {
                emp.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
