using ERPSys.SharedKernel.Events;
using ERPSys.SharedKernel.IntegrationEvents.HR;
using Finance.Application.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Finance.Infrastructure.Integration.RabbitMqConsumers
{
    public class PayrollPostedConsumer : BackgroundService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceScopeFactory _scopeFactory;

        public PayrollPostedConsumer(IConnectionFactory connectionFactory, IServiceScopeFactory scopeFactory)
        {
            _connectionFactory = connectionFactory;
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            var exchangeName = "hr_finance_exchange";
            var queueName = "finance_payroll_queue";

            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true);
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var envelope = JsonSerializer.Deserialize<EventEnvelope<PayrollCalculatedIntegrationEvent>>(json);

                    if (envelope?.Event != null)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<PayrollCalculatedHandler>();
                        await handler.HandleAsync(envelope.Event);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error processing PayrollCalculatedIntegrationEvent: {ex.Message}");
                }
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
