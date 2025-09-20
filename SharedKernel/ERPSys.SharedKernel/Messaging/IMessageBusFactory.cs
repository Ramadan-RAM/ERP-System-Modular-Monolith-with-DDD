using ERPSys.SharedKernel.Events;

namespace ERPSys.SharedKernel.Messaging
{
    /// <summary>
    /// Factory to resolve concrete publisher/subscriber by protocol.
    /// Implement this in Infrastructure to wire AMQP/MQTT/STOMP.
    /// </summary>
    public interface IMessageBusFactory
    {
        IEventPublisher GetPublisher(TransportProtocol protocol);
        IEventSubscriber GetSubscriber(TransportProtocol protocol);
    }
}
