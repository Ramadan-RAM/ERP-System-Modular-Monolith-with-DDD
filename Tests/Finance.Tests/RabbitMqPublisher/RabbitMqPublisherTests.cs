

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;


namespace Finance.Tests.RabbitMqPublisher
{

    public class RabbitMqPublisherTests
    {
        [Fact]
        public void Should_Publish_And_Consume_Message()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare exchange + queue
            channel.ExchangeDeclare("test_exchange", ExchangeType.Direct, durable: true);
            channel.QueueDeclare("test_queue", durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind("test_queue", "test_exchange", "test_key");

            var messageBody = Encoding.UTF8.GetBytes("Hello from Improved Unit Test");

            // Publish message
            channel.BasicPublish("test_exchange", "test_key", null, messageBody);

            // Prepare consumer
            var consumer = new EventingBasicConsumer(channel);
            string? consumedMessage = null;

            consumer.Received += (sender, ea) =>
            {
                consumedMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
            };

            channel.BasicConsume("test_queue", true, consumer);

            // Wait max 2 seconds for message
            var timeout = DateTime.UtcNow.AddSeconds(2);
            while (consumedMessage == null && DateTime.UtcNow < timeout)
            {
                System.Threading.Thread.Sleep(100);
            }

            Assert.NotNull(consumedMessage);
            Assert.Equal("Hello from Improved Unit Test", consumedMessage);
    }
    }


}
