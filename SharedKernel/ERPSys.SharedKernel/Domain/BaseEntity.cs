using ERPSys.SharedKernel.Events;

namespace ERPSys.SharedKernel.Domain
{
    /// <summary>
    /// EN: Generic base entity, supports any type of Id (int, Guid, string).
    /// 
    /// </summary>
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; protected set; }

        public DateTime CreatedOn { get;  set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get;  set; }

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();

        public void MarkUpdated() => UpdatedOn = DateTime.UtcNow;
    }
}

