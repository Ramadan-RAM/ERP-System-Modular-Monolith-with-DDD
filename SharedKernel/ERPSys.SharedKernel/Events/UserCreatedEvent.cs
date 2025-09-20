namespace ERPSys.SharedKernel.Events.IntegrationEvents
{
    public class UserCreatedEvent : IDomainEvent
    {
        public Guid UserId { get; set; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public UserCreatedEvent(Guid userId) => UserId = userId;
    }
}
