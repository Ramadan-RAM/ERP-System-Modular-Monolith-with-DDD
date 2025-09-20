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
    public class LeaveRepository : ILeaveRepository
    {
        private readonly HRDbContext _context;

        public LeaveRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmployeeLeave>> GetByEmployeeAndMonth(int employeeId, int year, int month)
        {
            return await _context.Leaves
                .Where(l => l.EmployeeId == employeeId &&
                            l.StartDate.Year == year &&
                            l.StartDate.Month == month)
                .ToListAsync();
        }
    }
}
