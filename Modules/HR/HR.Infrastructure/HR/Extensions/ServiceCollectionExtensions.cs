using ERPSys.SharedKernel.Events;
using ERPSys.SharedKernel.Messaging;
using ERPSys.SharedKernel.Persistence;
using HR.Application.HR.Interfaces;
using HR.Application.Interfaces;
using HR.Application.Interfaces.HR;
using HR.Application.Interfaces.InterfacRepository;
using HR.Application.InterfacRepository;
using HR.Infrastructure.EfRepository;
using HR.Infrastructure.HR.DbContext;
using HR.Infrastructure.HR.Services;
using HR.Infrastructure.HR.Services.ServicesRepository;
using HR.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.IntegrationEvents.HR;

namespace HR.Infrastructure.HR.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers HR services, repositories, and DbContext into the DI container.
        /// </summary>
        public static IServiceCollection AddHRServices(this IServiceCollection services, IConfiguration configuration)
        {
            // ----------------------
            // Application Services
            // ----------------------
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IJobTitleService, JobTitleService>();
            services.AddScoped<IEmployeeLeaveService, EmployeeLeaveService>();
            services.AddScoped<IPayslipService, PayslipService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IFingerprintDeviceService, FingerprintDeviceService>();

            // ----------------------
            // Specialized Services
            // ----------------------
            services.AddScoped<AttendanceSyncService>();

            // ----------------------
            // Repositories
            // ----------------------
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<ILeaveRepository, LeaveRepository>();
            services.AddScoped<IPayslipCalculatorService, PayslipCalculatorService>();
            services.AddScoped<IPayslipRepository, PayslipRepository>();
            services.AddScoped<IOutboxRepository, OutboxRepository>();

            // Generic Repository + Unit of Work
            services.AddScoped(typeof(IRepository<,>), typeof(HrRepository<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ----------------------
            // DbContext
            // ----------------------
            services.AddDbContext<HRDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("HRConnection")));

            return services;
        }
    }
}
