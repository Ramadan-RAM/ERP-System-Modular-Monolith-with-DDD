// File: Logging/Services/LoggingService.cs

using Logging.Application.Interfaces;
using Logging.Domain.Entities;
using Logging.Infrastructure.DbContext;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Logging.Infrastructure.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly LoggingDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggingService(LoggingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogErrorAsync(string module, string service, string action, Exception exception, object? requestData = null)
        {
            var log = new ErrorLog
            {
                Module = module,
                Service = service,
                Action = action,
                ErrorMessage = exception.Message,
                StackTrace = exception.StackTrace,
                RequestData = SerializeObject(requestData),
                Timestamp = DateTime.UtcNow
            };

            _dbContext.ErrorLogs.Add(log);
            await _dbContext.SaveChangesAsync();
        }

        private string? SerializeObject(object? data)
        {
            if (data == null) return null;
            try
            {
                return JsonSerializer.Serialize(data);
            }
            catch
            {
                return "⚠ Failed to serialize request data.";
            }
        }
    }
}
