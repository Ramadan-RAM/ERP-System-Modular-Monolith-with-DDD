// ============================================
// File: SharedKernel/Messaging/RabbitMqEventSubscriber.cs
// Namespace: ERPSys.SharedKernel.Messaging
// Purpose: Subscribe to events via RabbitMQ
// ============================================

using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ERPSys.SharedKernel.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ERPSys.SharedKernel.Messaging
{
    /// <summary>
    /// EN: RabbitMQ-based implementation of IEventSubscriber.
    /// </summary>
    public class RabbitMqEventSubscriber : IEventSubscriber
    {
        private readonly IConnectionFactory _connectionFactory;

        public RabbitMqEventSubscriber(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task SubscribeAsync<TEvent>(string topic, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent
        {
            var connection = _connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "erp.exchange", type: ExchangeType.Direct);
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName, exchange: "erp.exchange", routingKey: topic);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var envelope = JsonSerializer.Deserialize<EventEnvelope<TEvent>>(json);

                // TODO: raise callback/handler
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
