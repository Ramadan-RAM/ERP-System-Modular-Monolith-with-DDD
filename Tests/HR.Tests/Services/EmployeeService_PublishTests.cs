// HR.Tests/Services/EmployeeService_PublishTests.cs
// Test that EmployeeService creates an Outbox message when using RabbitMQ,
// and publishes directly when using the InMemory bus.

using AutoMapper;
using HR.Application.DTOs.HR;
using Moq;
using ERPSys.SharedKernel.Messaging;
using ERPSys.SharedKernel.Events;
using Microsoft.Extensions.Logging;
using HR.Infrastructure.Outbox;
using HR.Infrastructure.HR.Services;
using Logging.Application.Interfaces;
using HR.Infrastructure.HR.DbContext;

namespace HR.Tests.Services
{
    public class EmployeeService_PublishTests
    {
        // NOTE: these interfaces/classes are assumed present in your solution:
        // IOutboxRepository, IEventPublisher, IEmployeeRepository (or HRDbContext), ILoggingService, IMapper

        [Fact(DisplayName = "CreateAsync should add OutboxMessage when transport is RabbitMQ (Outbox)")]
        public async Task CreateAsync_ShouldAddOutboxMessage_WhenRabbitMqOutbox()
        {
            // Arrange
            var outboxRepoMock = new Mock<IOutboxRepository>();
            outboxRepoMock.Setup(r => r.AddAsync(It.IsAny<OutboxMessage>())).Returns(Task.CompletedTask);

            // mapper + other deps - minimal
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<EmployeeService>>();
            var logServiceMock = new Mock<ILoggingService?>();
            // create service with mocked dependencies (adjust constructor signature as in your project)
            // For this test we only assert that outboxRepo.AddAsync was called after CreateAsync

            // You will adapt construction depending on actual EmployeeService constructor.
            // Example pseudo:
            var service = new EmployeeService(
                /*context*/ Mock.Of<HRDbContext>(),
                mapperMock.Object,
                Mock.Of<ILogger<EmployeeService>>(),
                Mock.Of<ILoggingService>(),
                outboxRepoMock.Object,
                /*eventPublisher*/ null // service uses outbox when RabbitMQ configured
            );

            var dto = new EmployeeDTO
            {
                FirstName = "Test",
                MiddelName = "A",
                LastName = "User",
                Department = "IT",
                JobTitle = "Developer",
                BasicSalary = 1000m
            };

            // Act
            // call CreateAsync - adapt if method signature differs
            await service.CreateAsync(dto);

            // Assert
            outboxRepoMock.Verify(r => r.AddAsync(It.IsAny<OutboxMessage>()), Times.AtLeastOnce);
        }

        [Fact(DisplayName = "CreateAsync should publish immediately when using InMemoryEventBus")]
        public async Task CreateAsync_ShouldPublishInMemory_WhenInMemoryConfigured()
        {
            // Arrange
            var inMemoryBusMock = new Mock<IEventPublisher>(); // InMemoryEventBus implements IEventPublisher
            var mapperMock = new Mock<IMapper>();
            var logServiceMock = new Mock<ILoggingService>();
            // construct service with inMemoryBusMock as _eventPublisher
            var service = new EmployeeService(
                Mock.Of<HRDbContext>(),
                mapperMock.Object,
                Mock.Of<ILogger<EmployeeService>>(),
                logServiceMock.Object,
                Mock.Of<IOutboxRepository>(),
                inMemoryBusMock.Object // event publisher
            );

            var dto = new EmployeeDTO
            {
                FirstName = "Test",
                MiddelName = "A",
                LastName = "User",
                Department = "IT",
                JobTitle = "Developer",
                BasicSalary = 1000m
            };

            // Act
            await service.CreateAsync(dto);

            // Assert: publish called at least once
            inMemoryBusMock.Verify(m => m.PublishAsync(
                It.IsAny<IDomainEvent>(),
                It.IsAny<EventMetadata>(),
                default),
                Times.AtLeastOnce);
        }
    }
}
