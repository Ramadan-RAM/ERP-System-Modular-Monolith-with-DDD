using HR.Domain.HR.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.Interfaces
{
    public interface IPayslipCalculatorService
    {
        decimal CalculateDeductions(List<AttendanceRecord> attendanceRecords, List<EmployeeLeave> leaves, decimal baseSalary);
    }
}
