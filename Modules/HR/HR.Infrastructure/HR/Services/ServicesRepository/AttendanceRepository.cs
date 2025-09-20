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
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly HRDbContext _context;

        public AttendanceRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task<List<AttendanceRecord>> GetByEmployeeAndMonth(int employeeId, int year, int month)
        {
            return await _context.AttendanceRecords
                .Where(r => r.EmployeeId == employeeId &&
                            r.Date.Year == year &&
                            r.Date.Month == month)
                .ToListAsync();
        }
    }
}
