using ERPSys.SharedKernel.IntegrationEvents.HR;
using ERPSys.SharedKernel.Messaging;
using Finance.Application.Handlers;
using Finance.Infrastructure.Integration.RabbitMqConsumers;
using Finance.Infrastructure.Messaging;
using HR.Infrastructure.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using SharedKernel.IntegrationEvents.HR;

namespace Finance.Infrastructure.Extensions
{
    public static class MessagingSetupExtensions
    {
        public static IServiceCollection AddMessagingHrWithFinance(this IServiceCollection services, IConfiguration config)
        {
            var transport = config.GetValue<string>("Messaging:Transport");

            if (transport == "RabbitMQ")
            {
                // ✅ RabbitMQ Mode
                services.AddSingleton<IConnectionFactory>(sp =>
                {
                    var rabbitConfig = config.GetSection("Messaging:RabbitMQ");
                    return new ConnectionFactory
                    {
                        HostName = rabbitConfig["HostName"],
                        Port = int.Parse(rabbitConfig["Port"] ?? "5672"),
                        UserName = rabbitConfig["UserName"],
                        Password = rabbitConfig["Password"]
                    };
                });

                services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

                // Consumers
                services.AddHostedService<EmployeeConsumer>();
                services.AddHostedService<PayrollPostedConsumer>();

                // Handlers
                services.AddScoped<IIntegrationEventHandler<EmployeeCreatedIntegrationEvent>, EmployeeCreatedIntegrationEventHandler>();
                services.AddScoped<EmployeeCreatedIntegrationEventHandler>();

                services.AddScoped<IIntegrationEventHandler<EmployeeUpdatedIntegrationEvent>, EmployeeUpdatedIntegrationEventHandler>();
                services.AddScoped<EmployeeUpdatedIntegrationEventHandler>();

                services.AddScoped<IIntegrationEventHandler<PayrollCalculatedIntegrationEvent>, PayrollCalculatedHandler>();
                services.AddScoped<PayrollCalculatedHandler>();

                // Outbox Dispatcher
                services.AddHostedService<OutboxDispatcherBackgroundService>();
            }
            else if (transport == "InMemory")
            {
                // ✅ InMemory Mode
                var inMemoryBus = new InMemoryEventBus();
                services.AddSingleton<IEventPublisher>(inMemoryBus);

                // Handlers
                services.AddScoped<IIntegrationEventHandler<EmployeeCreatedIntegrationEvent>, EmployeeCreatedIntegrationEventHandler>();
                services.AddScoped<EmployeeCreatedIntegrationEventHandler>();

                services.AddScoped<IIntegrationEventHandler<EmployeeUpdatedIntegrationEvent>, EmployeeUpdatedIntegrationEventHandler>();
                services.AddScoped<EmployeeUpdatedIntegrationEventHandler>();

                services.AddScoped<IIntegrationEventHandler<PayrollCalculatedIntegrationEvent>, PayrollCalculatedHandler>();
                services.AddScoped<PayrollCalculatedHandler>();

                // Subscriptions
                services.AddSingleton<IHostedService>(sp =>
                {
                    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                    inMemoryBus.Subscribe<EmployeeCreatedIntegrationEvent>(async (@event, meta) =>
                    {
                        using var scope = scopeFactory.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<EmployeeCreatedIntegrationEventHandler>();
                        await handler.HandleAsync(@event);
                    });

                    inMemoryBus.Subscribe<EmployeeUpdatedIntegrationEvent>(async (@event, meta) =>
                    {
                        using var scope = scopeFactory.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<EmployeeUpdatedIntegrationEventHandler>();
                        await handler.HandleAsync(@event);
                    });

                    inMemoryBus.Subscribe<PayrollCalculatedIntegrationEvent>(async (@event, meta) =>
                    {
                        using var scope = scopeFactory.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<PayrollCalculatedHandler>();
                        await handler.HandleAsync(@event);
                    });

                    return new BackgroundServiceDummy();
                });
            }
            else if (transport == "Hybrid")
            {
                // ✅ Hybrid Mode
                services.AddSingleton<IConnectionFactory>(sp =>
                {
                    var rabbitConfig = config.GetSection("Messaging:RabbitMQ");
                    return new ConnectionFactory
                    {
                        HostName = rabbitConfig["HostName"],
                        Port = int.Parse(rabbitConfig["Port"] ?? "5672"),
                        UserName = rabbitConfig["UserName"],
                        Password = rabbitConfig["Password"]
                    };
                });

                services.AddSingleton<RabbitMqEventPublisher>();
                services.AddSingleton<InMemoryEventBus>();
                services.AddSingleton<IEventPublisher, HybridEventPublisher>();

                // Handlers
                services.AddScoped<IIntegrationEventHandler<EmployeeCreatedIntegrationEvent>, EmployeeCreatedIntegrationEventHandler>();
                services.AddScoped<EmployeeCreatedIntegrationEventHandler>();

                services.AddScoped<IIntegrationEventHandler<EmployeeUpdatedIntegrationEvent>, EmployeeUpdatedIntegrationEventHandler>();
                services.AddScoped<EmployeeUpdatedIntegrationEventHandler>();

                services.AddScoped<IIntegrationEventHandler<PayrollCalculatedIntegrationEvent>, PayrollCalculatedHandler>();
                services.AddScoped<PayrollCalculatedHandler>();

                // Consumers (RabbitMQ)
                services.AddHostedService<EmployeeConsumer>();
                services.AddHostedService<PayrollPostedConsumer>();

                // Outbox Dispatcher
                services.AddHostedService<OutboxDispatcherBackgroundService>();

                // Subscriptions (InMemory)
                services.AddSingleton<IHostedService>(sp =>
                {
                    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                    var inMemoryBus = sp.GetRequiredService<InMemoryEventBus>();

                    inMemoryBus.Subscribe<EmployeeCreatedIntegrationEvent>(async (@event, meta) =>
                    {
                        using var scope = scopeFactory.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<EmployeeCreatedIntegrationEventHandler>();
                        await handler.HandleAsync(@event);
                    });

                    inMemoryBus.Subscribe<EmployeeUpdatedIntegrationEvent>(async (@event, meta) =>
                    {
                        using var scope = scopeFactory.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<EmployeeUpdatedIntegrationEventHandler>();
                        await handler.HandleAsync(@event);
                    });

                    inMemoryBus.Subscribe<PayrollCalculatedIntegrationEvent>(async (@event, meta) =>
                    {
                        using var scope = scopeFactory.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<PayrollCalculatedHandler>();
                        await handler.HandleAsync(@event);
                    });

                    return new BackgroundServiceDummy();
                });
            }

            return services;
        }
    }
}
