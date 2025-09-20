using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Finance.Infrastructure.Persistence.DBContext
{
    // 🟢 Factory used by EF Core Migrations at design-time
    public class FinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
    {
        public FinanceDbContext CreateDbContext(string[] args)
        {
            // EF Core will look at the ERP.API project to find appsettings.json during migrations
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ERP.API"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("FinanceConnection");

            var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new FinanceDbContext(optionsBuilder.Options);
        }
    }
}
