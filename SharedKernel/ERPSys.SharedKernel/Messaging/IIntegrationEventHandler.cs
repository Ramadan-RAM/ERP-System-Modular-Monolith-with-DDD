namespace ERPSys.SharedKernel.Messaging
{
    public interface IIntegrationEventHandler<TEvent>
    {
        Task HandleAsync(TEvent @event);
    }
}
