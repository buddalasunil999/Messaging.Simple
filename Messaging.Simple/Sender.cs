using System.Text;
using RabbitMQ.Client;

namespace Messaging.Simple
{
    public class Sender : Connection
    {
        private readonly IMessageLogger messageLogger;
        private readonly ConnectionConfiguration connectionConfiguration;
        private readonly IBasicProperties properties;

        public Sender(IMessageLogger messageLogger,
            ConnectionConfiguration connectionConfiguration)
            : base(messageLogger, connectionConfiguration)
        {
            this.messageLogger = messageLogger;
            this.connectionConfiguration = connectionConfiguration;

            properties = Channel.CreateBasicProperties();
            properties.Persistent = true;
        }

        public void Send(string routingKey,
            string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            Channel.BasicPublish(exchange: connectionConfiguration.Exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            messageLogger.Info($" [x] Sent {message}");
        }
    }
}
