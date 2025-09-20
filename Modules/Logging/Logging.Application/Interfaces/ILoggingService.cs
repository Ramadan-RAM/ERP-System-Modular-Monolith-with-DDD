// File: Logging/Interfaces/ILoggingService.cs
namespace Logging.Application.Interfaces
{
    public interface ILoggingService
    {
        Task LogErrorAsync(string module, string service, string action, Exception exception, object? requestData = null);
    }
}
