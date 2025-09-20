namespace HR.Infrastructure.Outbox
{
    public interface IOutboxRepository
    {
        /// <summary>
        /// Add a new message to the Outbox (before sending).
        /// </summary>
        Task AddAsync(OutboxMessage msg);

        /// <summary>
        /// Fetch unsent messages (batch).
        /// </summary>
        Task<IEnumerable<OutboxMessage>> GetUnsentAsync(int max = 50);

        /// <summary>
        /// Update the message status after it has been tracked (mark as sent).
        /// </summary>
        Task MarkSentAsync(long id);

        
      
    }
}