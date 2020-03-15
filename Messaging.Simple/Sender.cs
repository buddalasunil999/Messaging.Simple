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

        public void Send(string routingKey,
            string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            var properties = Channel.CreateBasicProperties();
            properties.Persistent = true;

            Channel.ConfirmSelect();
            Channel.BasicPublish(exchange: connectionConfiguration.Exchange,
                routingKey: routingKey,
                basicProperties: properties,
                mandatory:true,
                body: body);
            Channel.WaitForConfirms();

            messageLogger.Info($" [x] Sent {message}");
        }
    }
}
