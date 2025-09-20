// File: Domain/Logging/Entities/ErrorLog.cs
namespace Logging.Domain.Entities
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Module { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? RequestData { get; set; }
    }
}
