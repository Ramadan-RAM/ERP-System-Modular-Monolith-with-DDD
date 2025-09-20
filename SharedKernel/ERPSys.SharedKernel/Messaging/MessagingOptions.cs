namespace ERPSys.SharedKernel.Messaging
{
    /// <summary>
    /// Options to choose the default transport protocol & default source name.
    /// </summary>
    public class MessagingOptions
    {
        public TransportProtocol DefaultProtocol { get; set; } = TransportProtocol.AMQP;
        public string Source { get; set; } = "ERP.Core"; // e.g., "HR", "Finance", etc.
    }
}
