using Finance.Domain.Common.Enums;
using Finance.Domain.Entities;
using Finance.Domain.Integrations.Links;
using Finance.Domain.ValueObjects;
using HR.Domain.HR.Entities;
using HR.Infrastructure.HR.DbContext;
using Logging.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Persistence.DBContext.Seed
{
    public static class FinanceDbSeed
    {
        public static void Seed(FinanceDbContext context, HRDbContext hrContext, ILoggingService logService)
        {
            try
            {
                var startYear = DateTime.UtcNow.Year - 5;
                var random = new Random();

                // ----------------------
                // Seed Currencies
                // ----------------------
                var currencies = new[]
                {
                    new Currency("USD", "US Dollar", true),
                    new Currency("EGP", "Egyptian Pound"),
                    new Currency("EUR", "Euro")
                };
                foreach (var c in currencies)
                    if (!context.Currencies.Any(x => x.Code == c.Code))
                        context.Currencies.Add(c);

                context.SaveChanges();

                // ----------------------
                // Seed Cost Centers from HR Departments
                // ----------------------
                var departments = hrContext.Departments.ToList();

                foreach (var dept in departments)
                {
                    if (!context.CostCenters.Any(c => c.DepartmentId == dept.Id))
                    {
                        var code = !string.IsNullOrWhiteSpace(dept.Name) && dept.Name.Length >= 3
                            ? dept.Name[..3].ToUpper()
                            : (dept.Name ?? "GEN").ToUpper();

                        context.CostCenters.Add(new CostCenter(code, dept.Name ?? "General", dept.Id));
                    }
                }

                // Fallback default cost centers if there are no HR departments
                if (!departments.Any())
                {
                    var defaultCenters = new[]
                    {
                        new CostCenter("ADM", "Administration"),
                        new CostCenter("SAL", "Sales"),
                        new CostCenter("PRD", "Production"),
                        new CostCenter("HR", "Human Resources")
                    };
                    foreach (var cc in defaultCenters)
                        if (!context.CostCenters.Any(c => c.Code == cc.Code))
                            context.CostCenters.Add(cc);
                }

                context.SaveChanges();

                // ----------------------
                // Seed General Ledger (GL) Accounts
                // ----------------------
                var accounts = new[]
                {
                    new GLAccount(new AccountCode("1001"), "Cash", AccountType.Asset, BalanceSide.Debit),
                    new GLAccount(new AccountCode("2001"), "Accounts Payable", AccountType.Liability, BalanceSide.Credit),
                    new GLAccount(new AccountCode("3001"), "Payroll Expense", AccountType.Expense, BalanceSide.Debit),
                    new GLAccount(new AccountCode("4001"), "Sales Revenue", AccountType.Revenue, BalanceSide.Credit),
                    new GLAccount(new AccountCode("5001"), "General Expenses", AccountType.Expense, BalanceSide.Debit)
                };
                foreach (var acc in accounts)
                    if (!context.GLAccounts.Any(a => a.Code.Value == acc.Code.Value))
                        context.GLAccounts.Add(acc);

                context.SaveChanges();

                var glAccounts = context.GLAccounts.ToList();
                var cashAccount = glAccounts.First(a => a.Code.Value == "1001");
                var payrollAccount = glAccounts.First(a => a.Code.Value == "3001");
                var revenueAccount = glAccounts.First(a => a.Code.Value == "4001");
                var generalExpAccount = glAccounts.First(a => a.Code.Value == "5001");

                // ----------------------
                // Link HR Departments to Finance Cost Centers
                // ----------------------
                var hrDepartments = hrContext.Employees
                  .Include(e => e.JobTitle)
                  .ThenInclude(j => j.Department).ToList();

                var costCenters = context.CostCenters.ToList();

                foreach (var emp in hrDepartments)
                {
                    var deptId = emp.JobTitle.Department.Id;
                    var deptName = emp.JobTitle.Department.Name;
                    var costCenterId = costCenters.First(c => c.DepartmentId == deptId).Id;

                    if (!context.DepartmentCostLinks.Any(l => l.DepartmentId == deptId && l.EmployeeCode == emp.EmployeeCode))
                    {
                        string fullName = $"{emp.FirstName} {(string.IsNullOrWhiteSpace(emp.MiddelName) ? "" : emp.MiddelName + " ")}{emp.LastName}";
                        var deptCostLink = new DepartmentCostLink(
                            deptId,
                            deptName,
                            emp.JobTitle.Title,
                            emp.EmployeeCode,
                            fullName,
                            emp.NetSalary,
                            costCenterId,
                            payrollAccount.Id
                        );
                        context.DepartmentCostLinks.Add(deptCostLink);
                    }
                }

                // ----------------------
                // Seed Exchange Rates
                // ----------------------
                for (int year = startYear; year <= DateTime.UtcNow.Year; year++)
                {
                    if (!context.ExchangeRates.Any(er => er.RateDate.Year == year))
                    {
                        context.ExchangeRates.AddRange(
                            new ExchangeRate("USD", "EGP", 15 + random.Next(20, 50), new DateTime(year, 3, 1)),
                            new ExchangeRate("EUR", "EGP", 18 + random.Next(25, 55), new DateTime(year, 6, 1)),
                            new ExchangeRate("USD", "EUR", 0.8m + (decimal)random.NextDouble() / 5, new DateTime(year, 9, 1))
                        );
                    }
                }
                context.SaveChanges();

                // ----------------------
                // Seed Financial Years + Related Data
                // ----------------------
                for (int year = startYear; year <= DateTime.UtcNow.Year; year++)
                {
                    if (!context.FinancialYears.Any(fy => fy.Year == year))
                    {
                        context.FinancialYears.Add(new FinancialYear(
                            year,
                            new Period(new DateTime(year, 1, 1), new DateTime(year, 12, 31))
                        ));
                    }

                    // General Expenses
                    if (!context.GeneralExpenses.Any(e => e.ExpenseDate.Year == year))
                    {
                        var expenses = new List<GeneralExpense>();
                        for (int month = 1; month <= 12; month++)
                        {
                            var baseDate = new DateTime(year, month, 15);
                            expenses.Add(new GeneralExpense(baseDate, ExpenseCategory.Utilities, new Money(random.Next(1000, 3000), "EGP"), generalExpAccount.Id, null, $"Electricity Bill - {month}/{year}"));
                            expenses.Add(new GeneralExpense(baseDate, ExpenseCategory.Rent, new Money(10000, "EGP"), generalExpAccount.Id, null, $"Office Rent - {month}/{year}"));
                            expenses.Add(new GeneralExpense(baseDate, ExpenseCategory.Marketing, new Money(random.Next(2000, 5000), "USD"), generalExpAccount.Id, null, $"Marketing Campaign - {month}/{year}"));
                            expenses.Add(new GeneralExpense(baseDate, ExpenseCategory.Payroll, new Money(50000 + random.Next(2000, 8000), "EGP"), payrollAccount.Id, null, $"Employee Salaries - {month}/{year}"));
                        }
                        context.GeneralExpenses.AddRange(expenses);
                    }

                    // Journal Entries
                    if (!context.JournalEntries.Any(j => j.EntryDate.Year == year))
                    {
                        var journals = new List<JournalEntry>();
                        int jeCounter = 1;

                        foreach (var dept in hrDepartments)
                        {
                            var costCenter = costCenters.FirstOrDefault(c => c.DepartmentId == dept.Id);

                            for (int month = 1; month <= 12; month++)
                            {
                                var date = new DateTime(year, month, 25);

                                // Sales Entry
                                var sales = new JournalEntry(new DocumentNumber($"JE-SALES-{year}-{jeCounter:D3}"), date);
                                sales.AddLine(new JournalLine(cashAccount.Id, new Money(10000 + month * 1000, "USD"), null, costCenter?.Id, "Cash from Sales"));
                                sales.AddLine(new JournalLine(revenueAccount.Id, null, new Money(10000 + month * 1000, "USD"), costCenter?.Id, "Sales Revenue"));
                                journals.Add(sales);

                                // Payroll Entry
                                var payroll = new JournalEntry(new DocumentNumber($"JE-PAYROLL-{year}-{jeCounter:D3}"), date);
                                payroll.AddLine(new JournalLine(payrollAccount.Id, new Money(50000, "EGP"), null, costCenter?.Id, "Payroll Expense"));
                                payroll.AddLine(new JournalLine(cashAccount.Id, null, new Money(50000, "EGP"), costCenter?.Id, "Cash"));
                                journals.Add(payroll);

                                jeCounter++;
                            }
                        }
                        context.JournalEntries.AddRange(journals);
                    }

                    // Purchase Orders
                    if (!context.PurchaseOrders.Any(p => p.OrderDate.Year == year))
                    {
                        var suppliers = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
                        int poCounter = 1;

                        for (int month = 1; month <= 12; month += 3)
                        {
                            var order = new PurchaseOrder(new DocumentNumber($"PO-{year}-{poCounter:D3}"), suppliers[poCounter % 3], new DateTime(year, month, 5), "USD");

                            order.AddItem(new PurchaseItem(Guid.NewGuid(), "Raw Material - Steel", InventoryItemType.RawMaterial, random.Next(50, 200), new Money(50, "USD"), new Money(80, "USD")));
                            order.AddItem(new PurchaseItem(Guid.NewGuid(), "Finished Good - Laptop", InventoryItemType.FinishedGood, random.Next(5, 20), new Money(900, "USD"), new Money(1200, "USD")));

                            context.PurchaseOrders.Add(order);
                            poCounter++;
                        }
                    }

                    // Inventory Snapshots
                    if (!context.InventorySnapshots.Any(s => s.AsOf.Year == year))
                    {
                        var snapshots = new List<InventoryItemSnapshot>();
                        for (int month = 1; month <= 12; month++)
                        {
                            var asOf = new DateTime(year, month, 20);

                            snapshots.Add(new InventoryItemSnapshot(Guid.NewGuid(), $"SKU-{year}-{month:D3}-N", $"Product-Normal-{year}-{month}", InventoryItemType.FinishedGood, random.Next(10, 50), new Money(100 + month * 5, "USD"), new Money(150 + month * 5, "USD"), null, asOf));

                            snapshots.Add(new InventoryItemSnapshot(Guid.NewGuid(), $"SKU-{year}-{month:D3}-NE", $"Product-NearExpiry-{year}-{month}", InventoryItemType.FinishedGood, random.Next(5, 30), new Money(80, "USD"), new Money(120, "USD"), new ExpiryInfo(asOf.AddDays(random.Next(1, 15)), true, 30), asOf));

                            snapshots.Add(new InventoryItemSnapshot(Guid.NewGuid(), $"SKU-{year}-{month:D3}-EX", $"Product-Expired-{year}-{month}", InventoryItemType.FinishedGood, random.Next(5, 20), new Money(70, "USD"), new Money(100, "USD"), new ExpiryInfo(asOf.AddDays(-random.Next(1, 30)), true, 30), asOf));
                        }
                        context.InventorySnapshots.AddRange(snapshots);
                    }

                    // Profit & Loss Forecasts
                    if (!context.ProfitLossForecasts.Any(f => f.Period.Start.Year == year))
                    {
                        var forecasts = new List<ProfitLossForecast>();
                        for (int month = 1; month <= 12; month++)
                        {
                            var start = new DateTime(year, month, 1);
                            var end = start.AddMonths(1).AddDays(-1);

                            forecasts.Add(new ProfitLossForecast(new Period(start, end), new Money(5000 + month * 500, "USD"), new Money(1000 + month * 100, "USD"), $"Forecast - {month}/{year}"));
                        }

                        forecasts.Add(new ProfitLossForecast(new Period(new DateTime(year, 1, 1), new DateTime(year, 12, 31)), new Money(60000, "USD"), new Money(12000, "USD"), $"Annual Forecast {year}"));

                        context.ProfitLossForecasts.AddRange(forecasts);
                    }
                }

                context.SaveChanges();

                // ----------------------
                // Processed Event Logs (mark that seeding ran)
                // ----------------------
                if (!context.ProcessedEventLogs.Any(l => l.HandlerName == "SeedInitialization"))
                {
                    context.ProcessedEventLogs.Add(new ProcessedEventLog(Guid.NewGuid(), "SeedInitialization"));
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                logService.LogErrorAsync(
                    "Finance",
                    nameof(FinanceDbSeed),
                    nameof(Seed),
                    ex,
                    null
                ).GetAwaiter().GetResult();

                throw;
            }
        }
    }
}
