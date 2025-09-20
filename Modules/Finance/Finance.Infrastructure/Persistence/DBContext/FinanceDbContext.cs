using Finance.Domain.Entities;
using Finance.Domain.Integrations.Links;
using Finance.Infrastructure.Persistence.Configurations;
using Finance.Infrastructure.Persistence.DBContext.Seed;
using HR.Infrastructure.HR.DbContext;
using Logging.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Finance.Infrastructure.Persistence.DBContext
{
    public class FinanceDbContext : DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

        // 🟢 Database Tables (DbSets)
        public DbSet<GLAccount> GLAccounts => Set<GLAccount>();
        public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
        public DbSet<JournalLine> JournalLines => Set<JournalLine>();
        public DbSet<Currency> Currencies => Set<Currency>();
        public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
        public DbSet<GeneralExpense> GeneralExpenses => Set<GeneralExpense>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
        public DbSet<InventoryItemSnapshot> InventorySnapshots => Set<InventoryItemSnapshot>();
        public DbSet<ProfitLossForecast> ProfitLossForecasts => Set<ProfitLossForecast>();
        public DbSet<FinancialYear> FinancialYears => Set<FinancialYear>();
        public DbSet<CostCenter> CostCenters => Set<CostCenter>();

        // 🟢 Integration with HR via DepartmentCostLink
        public DbSet<DepartmentCostLink> DepartmentCostLinks => Set<DepartmentCostLink>();
        public DbSet<ProcessedEventLog> ProcessedEventLogs => Set<ProcessedEventLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Load all EntityTypeConfiguration classes automatically
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        // 🟢 Helper method to run Seed data initialization
        public void SeedData(HRDbContext hrContext, ILoggingService logService)
          => FinanceDbSeed.Seed(this, hrContext, logService);
    }
}
