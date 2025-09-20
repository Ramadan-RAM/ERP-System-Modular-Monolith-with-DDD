using Finance.Domain;
using Finance.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Finance.Infrastructure.Extensions
{
    /// <summary>
    /// 🔹 Extension methods for registering Finance Module services and dependencies
    /// - Auto-registers all services ending with 'Service' as their implemented interfaces.
    /// - Auto-registers all repositories ending with 'Repository' as their implemented interfaces.
    /// - Registers generic repository (Infrastructure Layer).
    /// - Configures FinanceDbContext with SQL Server.
    /// 
    /// Usage:
    /// builder.Services.AddFinanceScanServices(builder.Configuration);
    /// </summary>
    public static class ServiceCollectionScanServicesExtensions
    {
        /// <summary>
        /// Adds Finance-related services, repositories, and DbContext to the DI container.
        /// Uses Scrutor scanning for auto-registration of services & repositories.
        /// </summary>
        public static IServiceCollection AddFinanceScanServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 📌 Auto-register all services that end with 'Service'
            services.Scan(scan => scan
                .FromAssemblyOf<FinanceDomainAssemblyReference>()
                .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            // 📌 Auto-register all repositories that end with 'Repository'
            services.Scan(scan => scan
                .FromAssemblyOf<FinanceDbContext>() // 👈 The EfRepository is usually located in Infrastructure
                .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            //// Example: Specific repository registration
            //services.AddScoped<IRepository<GeneralExpense>, EfRepository<GeneralExpense>>();

            //services.AddScoped<IUnitOfWork, UnitOfWork>();


            //// 📌 Generic Repository (Fallback)
            //services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            // 📌 DbContext
            services.AddDbContext<FinanceDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("FinanceConnection")));

            return services;
        }
    }
}
