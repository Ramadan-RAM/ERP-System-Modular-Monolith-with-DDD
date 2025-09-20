// File: Finance.Infrastructure/Messaging/EmployeeConsumer.cs
using ERPSys.SharedKernel.Events;
using ERPSys.SharedKernel.IntegrationEvents.HR;
using Finance.Application.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedKernel.IntegrationEvents.HR;
using System.Text;
using System.Text.Json;

namespace Finance.Infrastructure.Messaging
{
    public class EmployeeConsumer : BackgroundService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _exchangeName;

        public EmployeeConsumer(IConnectionFactory connectionFactory, IServiceScopeFactory scopeFactory, IConfiguration config)
        {
            _connectionFactory = connectionFactory;
            _scopeFactory = scopeFactory;
            _exchangeName = config.GetValue<string>("Messaging:ExchangeName") ?? "erp.exchange";
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            var queueName = "finance_employee_queue";

            channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Fanout, durable: true);
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                    // Try to specify the event type using $type or JSON properties.
                    if (json.Contains(nameof(EmployeeCreatedIntegrationEvent)))
                    {
                        var envelope = JsonSerializer.Deserialize<EventEnvelope<EmployeeCreatedIntegrationEvent>>(json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (envelope?.Event != null)
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var handler = scope.ServiceProvider.GetRequiredService<EmployeeCreatedIntegrationEventHandler>();
                            await handler.HandleAsync(envelope.Event);
                            Console.WriteLine($"✅ Processed EmployeeCreatedIntegrationEvent for {envelope.Event.FullName}");
                        }
                    }
                    else if (json.Contains(nameof(EmployeeUpdatedIntegrationEvent)))
                    {
                        var envelope = JsonSerializer.Deserialize<EventEnvelope<EmployeeUpdatedIntegrationEvent>>(json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (envelope?.Event != null)
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var handler = scope.ServiceProvider.GetRequiredService<EmployeeUpdatedIntegrationEventHandler>();
                            await handler.HandleAsync(envelope.Event);
                            Console.WriteLine($"✅ Processed EmployeeUpdatedIntegrationEvent for {envelope.Event.FullName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Received unknown event type.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error processing Employee event: {ex.Message}");
                }
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
