using ERPSys.SharedKernel.Messaging;
using ERPSys.SharedKernel.Persistence;
using Finance.Application.Handlers;
using Finance.Application.Interfaces;
using Finance.Application.ProcessedEvents;
using Finance.Application.Services;
using Finance.Infrastructure.Integration.Services;
using Finance.Infrastructure.Persistence;
using Finance.Infrastructure.Persistence.DBContext;
using Finance.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.IntegrationEvents.HR;

namespace Finance.Infrastructure.Extensions
{
    /// <summary>
    /// 🔹 Extension methods for registering Finance Module services and dependencies
    /// - Registers all Finance services (Application Layer).
    /// - Registers generic repository (Infrastructure Layer).
    /// - Configures FinanceDbContext with SQL Server.
    /// 
    /// Usage:
    /// builder.Services.AddFinanceServices(builder.Configuration);
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Finance-related services, repositories, and DbContext to the DI container.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="configuration">The application configuration (used for connection strings).</param>
        /// <returns>The IServiceCollection with Finance services registered.</returns>
        public static IServiceCollection AddFinanceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 📌 Application Services
            services.AddScoped<IChartReportService, ChartReportService>();
            services.AddScoped<IComparisonReportService, ComparisonReportService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IExchangeRateService, ExchangeRateService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IFinancialReportService, FinancialReportService>();
            services.AddScoped<IInventoryForecastService, InventoryForecastService>();
            services.AddScoped<IInventorySnapshotService, InventorySnapshotService>();
            services.AddScoped<IJournalService, JournalService>();
            services.AddScoped<IProfitLossForecastService, ProfitLossForecastService>();
            services.AddScoped<IPurchaseService, PurchaseService>();

            // 📌 UoW
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 📌 Account Mapping
            services.AddScoped<IAccountMappingService, AccountMappingService>();

            // 📌 Application services
            services.AddScoped<IGLAccountService, GLAccountService>();
            services.AddScoped<IFinanceOnboardingService, FinanceOnboardingService>();

            // 📌 Processed Events
            services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();

            services.AddScoped<IIntegrationEventHandler<EmployeeCreatedIntegrationEvent>, EmployeeCreatedIntegrationEventHandler>();

            services.AddScoped<PayrollCalculatedHandler>();

            // 📌 Generic repository (covers all aggregates)
            services.AddScoped(typeof(IRepository<,>), typeof(FinanceRepository<,>));

            // 📌 DbContext
            services.AddDbContext<FinanceDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("FinanceConnection")));

            return services;
        }
    }
}
