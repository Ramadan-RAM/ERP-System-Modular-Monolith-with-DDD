// File: Logging/LoggingDbContext.cs
using Microsoft.EntityFrameworkCore;
using Logging.Domain.Entities;

namespace Logging.Infrastructure.DbContext
{
    public class LoggingDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options) { }

        public DbSet<ErrorLog> ErrorLogs { get; set; }
    }
}
