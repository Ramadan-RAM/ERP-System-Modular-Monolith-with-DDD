using HR.Application.Interfaces;
using HR.Domain.HR.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Infrastructure.HR.Services
{
  
    public class PayslipCalculatorService : IPayslipCalculatorService
    {
        public decimal CalculateDeductions(List<AttendanceRecord> attendanceRecords, List<EmployeeLeave> leaves, decimal baseSalary)
        {
            decimal totalDeduction = 0;

            var workingDaysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int daysPresent = attendanceRecords.Count(ar => ar.TimeIn.HasValue && ar.TimeOut.HasValue);
            int daysOnLeave = leaves.Count;

            int totalWorked = daysPresent + daysOnLeave;
            int daysAbsent = workingDaysInMonth - totalWorked;

            decimal dailySalary = baseSalary / workingDaysInMonth;
            totalDeduction = daysAbsent * dailySalary;

            return totalDeduction;
        }
    }

}
