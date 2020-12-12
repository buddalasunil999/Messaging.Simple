using Newtonsoft.Json;

namespace Messaging.Simple
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly IMessageLogger messageLogger;
        private readonly RabbitMqConfiguration connectionConfiguration;

        public MessageDispatcher(IMessageLogger messageLogger,
            RabbitMqConfiguration connectionConfiguration)
        {
            this.messageLogger = messageLogger;
            this.connectionConfiguration = connectionConfiguration;
        }

        public void Send(string routingKey, string message, string exchange)
        {
            using (var sender = new Sender(messageLogger, connectionConfiguration))
            {
                sender.Send(routingKey, message, exchange);
            }
        }

        public void Send(string routingKey, object obj, string exchange)
        {
            Send(routingKey, JsonConvert.SerializeObject(obj), exchange);
        }

        public void Send(string routingKey, object obj)
        {
            Send(routingKey, obj, connectionConfiguration.Exchange);
        }

        public void Send<T>(T obj, string exchange)
        {
            Send(typeof(T).ToString(), obj, exchange);
        }

        public void Send<T>(T obj)
        {
            Send(typeof(T).ToString(), obj, connectionConfiguration.Exchange);
        }
    }
}