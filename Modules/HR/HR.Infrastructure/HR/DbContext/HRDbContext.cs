using HR.Domain.HR.Entities;
using HR.Infrastructure.HR.Configurations;
using HR.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.HR.DbContext
{
    public class HRDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public HRDbContext(DbContextOptions<HRDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<JobTitle> JobTitles => Set<JobTitle>();
        public DbSet<EmployeeLeave> Leaves => Set<EmployeeLeave>();

        public DbSet<Payslip> Payslips => Set<Payslip>();

       
        public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();

        public DbSet<Branch> Branches { get; set; }
        public DbSet<FingerprintDevice> FingerprintDevices => Set<FingerprintDevice>();

        // 🆕 Outbox
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new JobTitleConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeLeaveConfiguration());
            modelBuilder.ApplyConfiguration(new PayslipConfiguration());

            modelBuilder.ApplyConfiguration(new AttendanceRecordConfiguration());

            // ✅ New - Linking branches and devices
            modelBuilder.ApplyConfiguration(new BranchConfiguration());
            modelBuilder.ApplyConfiguration(new FingerprintDeviceConfiguration());

            modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());

        }
    }
}
