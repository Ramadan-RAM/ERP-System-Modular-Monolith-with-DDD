using ERPSys.SharedKernel.Events;
using Microsoft.Extensions.Logging;

namespace ERPSys.SharedKernel.Messaging
{
    /// <summary>
    /// Hybrid publisher: tries RabbitMQ first, falls back to InMemory if RabbitMQ fails.
    /// Use in Demo/Hybrid mode.
    /// </summary>
    public class HybridEventPublisher : IEventPublisher
    {
        private readonly RabbitMqEventPublisher _rabbitPublisher;
        private readonly InMemoryEventBus _inMemoryPublisher;
        private readonly ILogger<HybridEventPublisher> _logger;

        public HybridEventPublisher(
            RabbitMqEventPublisher rabbitPublisher,
            InMemoryEventBus inMemoryPublisher,
            ILogger<HybridEventPublisher> logger)
        {
            _rabbitPublisher = rabbitPublisher;
            _inMemoryPublisher = inMemoryPublisher;
            _logger = logger;
        }

        public async Task PublishAsync<TEvent>(
            TEvent @event,
            EventMetadata metadata,
            CancellationToken cancellationToken = default) where TEvent : IDomainEvent
        {
            try
            {
                await _rabbitPublisher.PublishAsync(@event, metadata, cancellationToken);
                _logger.LogInformation("✅ Published via RabbitMQ: {EventType}", typeof(TEvent).Name);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ RabbitMQ failed, fallback to InMemory for {EventType}", typeof(TEvent).Name);
                await _inMemoryPublisher.PublishAsync(@event, metadata, cancellationToken);
            }
        }
    }
}
