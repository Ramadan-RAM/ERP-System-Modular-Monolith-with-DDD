namespace ERPSys.SharedKernel.Messaging
{
    /// <summary>
    /// Supported transport protocols.
    /// AMQP  : Reliable broker-based messaging (RabbitMQ/MassTransit) - ERP, Banking, Core TX.
    /// MQTT  : Lightweight pub/sub for IoT/edge devices - sensors, weak networks.
    /// STOMP : Simple text protocol often over WebSockets - web apps, chat, notifications.
    /// </summary>
    public enum TransportProtocol
    {
        None = 0,
        AMQP = 1,   // RabbitMQ / MassTransit
        MQTT = 2,   // MQTTnet
        STOMP = 3   // WebSockets STOMP client/server
    }
}
