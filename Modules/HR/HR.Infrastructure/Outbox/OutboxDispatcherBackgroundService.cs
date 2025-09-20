using ERPSys.SharedKernel.Events;
using ERPSys.SharedKernel.Messaging;
using Logging.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HR.Infrastructure.Outbox
{
    public class OutboxDispatcherBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxDispatcherBackgroundService> _logger;

        public OutboxDispatcherBackgroundService(IServiceScopeFactory scopeFactory,ILogger<OutboxDispatcherBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 OutboxDispatcher started...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
                    var logService = scope.ServiceProvider.GetRequiredService<ILoggingService>();

                    var unsent = (await repo.GetUnsentAsync()).ToList();
                    if (!unsent.Any())
                    {
                        _logger.LogDebug("⏳ No unsent Outbox messages found");
                    }

                    foreach (var msg in unsent)
                    {
                        try
                        {
                            _logger.LogInformation("📦 Processing OutboxMessage {Id} - Type={Type} Payload={Payload}",
                                msg.Id, msg.Type, msg.Payload);

                            // 1️⃣ Resolve type
                            var evtType = Type.GetType(msg.Type)
                                ?? AppDomain.CurrentDomain
                                    .GetAssemblies()
                                    .SelectMany(a => a.GetTypes())
                                    .FirstOrDefault(t => t.FullName == msg.Type.Split(',')[0]);

                            if (evtType == null)
                            {
                                _logger.LogWarning("❌ Could not resolve event type: {Type}", msg.Type);
                                continue;
                            }

                            // 2️⃣ Deserialize JSON
                            var evt = JsonSerializer.Deserialize(msg.Payload, evtType, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                            if (evt == null)
                            {
                                _logger.LogWarning("❌ Failed to deserialize OutboxMessage {Id}", msg.Id);
                                continue;
                            }

                            // 3️⃣ Metadata
                            var metadata = new EventMetadata
                            {
                                EventId = msg.EventId,
                                EventType = msg.Type,
                                OccurredOn = msg.OccurredOn,
                                SourceModule = "HR"
                            };

                            _logger.LogInformation("📤 Publishing event {EventId} ({EventType})", msg.EventId, msg.Type);

                            // 4️⃣ Publish
                            await publisher.PublishAsync((dynamic)evt, metadata, stoppingToken);

                            // 5️⃣ Mark as sent
                            using var updateScope = _scopeFactory.CreateScope();
                            var updateRepo = updateScope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                            await updateRepo.MarkSentAsync(msg.Id);

                            _logger.LogInformation("✅ OutboxMessage {Id} dispatched successfully", msg.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "❌ Failed dispatching OutboxMessage {Id}", msg.Id);
                            await logService.LogErrorAsync("HR", "OutboxDispatcher", "Dispatch", ex, msg);
                        }
                    }
                }
                catch (Exception exOuter)
                {
                    _logger.LogError(exOuter, "❌ Error in Outbox dispatcher loop");

                    using var scope = _scopeFactory.CreateScope();
                    var logService = scope.ServiceProvider.GetRequiredService<ILoggingService>();
                    await logService.LogErrorAsync("HR", "OutboxDispatcher", nameof(ExecuteAsync), exOuter, null);
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
