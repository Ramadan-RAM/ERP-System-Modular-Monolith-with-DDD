using HR.Domain.HR.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.Interfaces.InterfacRepository
{
    public interface IAttendanceRepository
    {
        Task<List<AttendanceRecord>> GetByEmployeeAndMonth(int employeeId, int year, int month);
    }
}
