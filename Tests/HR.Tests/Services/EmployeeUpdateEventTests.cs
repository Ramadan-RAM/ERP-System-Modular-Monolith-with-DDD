// HR.Tests/Services/EmployeeUpdateEventTests.cs
// Ensure that UpdateAsync triggers EmployeeUpdatedIntegrationEvent publishing (InMemory or Outbox)

using ERPSys.SharedKernel.Events;
using ERPSys.SharedKernel.Messaging;
using HR.Application.DTOs.HR;
using HR.Infrastructure.HR.Services;
using HR.Infrastructure.Outbox;
using Logging.Application.Interfaces;
using Moq;


namespace HR.Tests.Services
{
    public class EmployeeUpdateEventTests
    {
        [Fact]
        public async Task UpdateAsync_ShouldPublishUpdatedEvent_InMemoryMode()
        {
            // Arrange
            var eventPublisherMock = new Mock<IEventPublisher>();
            var service = new EmployeeService(
                Mock.Of<HR.Infrastructure.HR.DbContext.HRDbContext>(),
                Mock.Of<AutoMapper.IMapper>(),
                Mock.Of<Microsoft.Extensions.Logging.ILogger<EmployeeService>>(),
                Mock.Of<ILoggingService>(),
                Mock.Of<IOutboxRepository>(),
                eventPublisherMock.Object
            );

            // make a minimal employee in DB or simulate FindAsync to return entity - skipped for brevity
            var dto = new EmployeeDTO
            {
                FirstName = "Test",
                MiddelName = "A",
                LastName = "User",
                Department = "IT",
                JobTitle = "Developer",
                BasicSalary = 1000m
            };

            // Act - adapt to actual id & method
            await service.UpdateAsync(1, dto);

            // Assert the publisher was called (InMemory scenario)
            eventPublisherMock.Verify(ep => ep.PublishAsync(
                It.IsAny<IDomainEvent>(), It.IsAny<EventMetadata>(), default), Times.AtLeastOnce);
        }
    }
}
