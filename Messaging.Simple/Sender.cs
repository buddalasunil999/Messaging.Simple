using RabbitMQ.Client;
using System.Text;

namespace Messaging.Simple
{
    public class Sender : Connection
    {
        private readonly IMessageLogger messageLogger;

        public Sender(IMessageLogger messageLogger,
            ConnectionConfiguration connectionConfiguration)
            : base(messageLogger, connectionConfiguration)
        {
            this.messageLogger = messageLogger;
        }

        public virtual void Send(string routingKey,
            string message)
        {
            var properties = Channel.CreateBasicProperties();
            properties.Persistent = true;

            Send(routingKey,
                message,
                properties,
                connectionConfiguration.Exchange);
        }

        public virtual void Send(string routingKey,
            string message,
            IBasicProperties properties,
            string exchange)
        {
            var body = Encoding.UTF8.GetBytes(message);

            Channel.ConfirmSelect();
            Channel.BasicPublish(exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                mandatory: true,
                body: body);
            Channel.WaitForConfirms();

            messageLogger.Info($" [x] Sent {message}");
        }
    }
}
