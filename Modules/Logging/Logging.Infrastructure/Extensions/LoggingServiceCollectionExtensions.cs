// File: Infrastructure/Logging/Extensions/LoggingServiceCollectionExtensions.cs
using Logging.Application.Interfaces;
using Logging.Infrastructure.Extensions;
using Logging.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Logging.Infrastructure.Extensions
{
    public static class LoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddLoggingServices(this IServiceCollection services)
        {
            services.AddScoped<ILoggingService, LoggingService>();
            return services;
        }
    }
}
