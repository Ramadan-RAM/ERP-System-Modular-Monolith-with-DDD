// ============================================
// File: SharedKernel/Messaging/RabbitMqEventPublisher.cs
// Namespace: ERPSys.SharedKernel.Messaging
// Purpose: Publish events via RabbitMQ (singleton connection)
// ============================================

using ERPSys.SharedKernel.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ERPSys.SharedKernel.Messaging
{
    /// <summary>
    /// EN: RabbitMQ-based implementation of IEventPublisher (singleton connection).
    /// </summary>
    public class RabbitMqEventPublisher : IEventPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMqEventPublisher> _logger;
        private bool _disposed;

        private readonly string _exchangeName;

        public RabbitMqEventPublisher(IConnectionFactory connectionFactory, ILogger<RabbitMqEventPublisher> logger, IConfiguration config)
        {
            _logger = logger;
            _connection = connectionFactory.CreateConnection();

            _exchangeName = config.GetValue<string>("Messaging:ExchangeName") ?? "erp.exchange";

            // ✅ Ensure the exchange exists once at startup
            using var channel = _connection.CreateModel();
            channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false
            );
        }

        public Task PublishAsync<TEvent>(
            TEvent @event,
            EventMetadata metadata,
            CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RabbitMqEventPublisher));

            using var channel = _connection.CreateModel();

            // Wrap event with metadata
            var envelope = new EventEnvelope<TEvent>(@event, metadata);

            var json = JsonSerializer.Serialize(envelope, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            var body = Encoding.UTF8.GetBytes(json);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            // RoutingKey = EventType (optional: could be empty for Fanout)
            var routingKey = typeof(TEvent).Name;

            channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body
            );

            _logger.LogInformation(
                "📤 Published {EventType} (EventId={EventId}) to Exchange={Exchange} [RoutingKey={RoutingKey}]",
                typeof(TEvent).Name, metadata.EventId, _exchangeName, routingKey);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _connection?.Dispose();
            _disposed = true;
        }
    }
}
