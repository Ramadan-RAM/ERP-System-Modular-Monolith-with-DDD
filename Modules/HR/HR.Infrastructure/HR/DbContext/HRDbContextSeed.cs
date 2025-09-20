using HR.Domain.HR.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.HR.DbContext
{
    public static class HRDbContextSeed
    {
        public static async Task SeedAsync(HRDbContext context)
        {
            // ----------------------
            // Seed Departments
            // ----------------------
            if (!context.Departments.Any())
            {
                context.Departments.AddRange(
                    new Department { Name = "IT" },         // Id = 1
                    new Department { Name = "HR" },         // Id = 2
                    new Department { Name = "Finance" },    // Id = 3
                    new Department { Name = "Marketing" }   // Id = 4
                );
            }

            // ----------------------
            // Seed Job Titles
            // ----------------------
            if (!context.JobTitles.Any())
            {
                context.JobTitles.AddRange(
                    new JobTitle { Title = "Software Developer", Description = "Responsible for writing code", BaseSalary = 10000, DepartmentId = 1 },
                    new JobTitle { Title = "HR Manager", Description = "Handles HR operations", BaseSalary = 9000, DepartmentId = 2 },
                    new JobTitle { Title = "Accountant", Description = "Handles financial records", BaseSalary = 9500, DepartmentId = 3 },
                    new JobTitle { Title = "Marketing Executive", Description = "Handles promotions", BaseSalary = 8500, DepartmentId = 4 }
                );
            }

            // ----------------------
            // Seed Branches
            // ----------------------
            if (!context.Branches.Any())
            {
                context.Branches.AddRange(
                    new Branch { Name = "Head Office" },
                    new Branch { Name = "Alex Branch" },
                    new Branch { Name = "Giza Branch" },
                    new Branch { Name = "Remote" }
                );
            }

            await context.SaveChangesAsync();

            // ----------------------
            // Seed Employees
            // ----------------------
            if (!context.Employees.Any())
            {
                var itDept = await context.Departments.FirstAsync(d => d.Name == "IT");
                var hrDept = await context.Departments.FirstAsync(d => d.Name == "HR");
                var financeDept = await context.Departments.FirstAsync(d => d.Name == "Finance");
                var marketingDept = await context.Departments.FirstAsync(d => d.Name == "Marketing");

                var devJob = await context.JobTitles.FirstAsync(j => j.Title == "Software Developer");
                var hrJob = await context.JobTitles.FirstAsync(j => j.Title == "HR Manager");
                var accountantJob = await context.JobTitles.FirstAsync(j => j.Title == "Accountant");
                var marketingJob = await context.JobTitles.FirstAsync(j => j.Title == "Marketing Executive");

                var emp1 = new Employee
                {
                    EmployeeCode = "EMP-HWX869",
                    Gender = "Male",
                    BasicSalary = 10000,
                    Allowances = 1000,
                    Deductions = 500
                };
                emp1.SetName("Ahmed", "Mohamed", "Ali");
                emp1.SetBirthDate(new DateTime(1990, 5, 12));
                emp1.AssignDepartmentAndJobTitle(itDept, devJob);

                var emp2 = new Employee
                {
                    EmployeeCode = "EMP-QJY870",
                    Gender = "Female",
                    BasicSalary = 9000,
                    Allowances = 900,
                    Deductions = 400
                };
                emp2.SetName("Sara", "Mohsen", "Ali");
                emp2.SetBirthDate(new DateTime(1992, 3, 21));
                emp2.AssignDepartmentAndJobTitle(hrDept, hrJob);

                var emp3 = new Employee
                {
                    EmployeeCode = "EMP-UPJ102",
                    Gender = "Male",
                    BasicSalary = 9500,
                    Allowances = 950,
                    Deductions = 350
                };
                emp3.SetName("Omar", "Said", "Youssef");
                emp3.SetBirthDate(new DateTime(1988, 8, 9));
                emp3.AssignDepartmentAndJobTitle(financeDept, accountantJob);

                var emp4 = new Employee
                {
                    EmployeeCode = "EMP-ODN152",
                    Gender = "Female",
                    BasicSalary = 8500,
                    Allowances = 800,
                    Deductions = 300
                };
                emp4.SetName("Laila", "Hossam", "Ibrahim");
                emp4.SetBirthDate(new DateTime(1995, 1, 5));
                emp4.AssignDepartmentAndJobTitle(marketingDept, marketingJob);

                context.Employees.AddRange(emp1, emp2, emp3, emp4);
                await context.SaveChangesAsync();
            }

            // ----------------------
            // Seed Attendance Records
            // ----------------------
            if (!context.AttendanceRecords.Any())
            {
                context.AttendanceRecords.AddRange(
                    new AttendanceRecord { EmployeeId = 1, Date = DateTime.Today, TimeIn = new TimeSpan(8, 0, 0), TimeOut = new TimeSpan(16, 0, 0), Source = "Manual", BranchId = 1 },
                    new AttendanceRecord { EmployeeId = 2, Date = DateTime.Today, TimeIn = new TimeSpan(8, 15, 0), TimeOut = new TimeSpan(15, 45, 0), Source = "Device", BranchId = 2 },
                    new AttendanceRecord { EmployeeId = 3, Date = DateTime.Today, TimeIn = new TimeSpan(9, 0, 0), TimeOut = new TimeSpan(17, 0, 0), Source = "Excel", BranchId = 1 },
                    new AttendanceRecord { EmployeeId = 4, Date = DateTime.Today, TimeIn = new TimeSpan(8, 5, 0), TimeOut = new TimeSpan(16, 10, 0), Source = "Device", BranchId = 3 }
                );
                await context.SaveChangesAsync();
            }

            // ----------------------
            // Seed Fingerprint Devices
            // ----------------------
            if (!context.FingerprintDevices.Any())
            {
                context.FingerprintDevices.AddRange(
                    new FingerprintDevice { Name = "ZKTeco1", IP = "192.168.1.10", Type = "ZKTeco", BranchId = 1 },
                    new FingerprintDevice { Name = "ZKTeco2", IP = "192.168.1.11", Type = "ZKTeco", BranchId = 2 }
                );
                await context.SaveChangesAsync();
            }

            // ----------------------
            // Seed Leaves
            // ----------------------
            if (!context.Leaves.Any())
            {
                context.Leaves.AddRange(
                    new EmployeeLeave { EmployeeId = 1, LeaveType = LeaveType.Annual, StartDate = DateTime.Today.AddDays(-5), EndDate = DateTime.Today, Reason = "Family vacation" },
                    new EmployeeLeave { EmployeeId = 2, LeaveType = LeaveType.Sick, StartDate = DateTime.Today.AddDays(-3), EndDate = DateTime.Today, Reason = "Flu" }
                );
                await context.SaveChangesAsync();
            }

            // ----------------------
            // Seed Payslips
            // ----------------------
            if (!context.Payslips.Any())
            {
                context.Payslips.AddRange(
                    new Payslip { EmployeeId = 1, PayDate = DateTime.Today, BasicSalary = 10000, Allowances = 1000, Deductions = 500, Notes = "Regular pay" },
                    new Payslip { EmployeeId = 2, PayDate = DateTime.Today, BasicSalary = 9000, Allowances = 900, Deductions = 400, Notes = "On time" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
