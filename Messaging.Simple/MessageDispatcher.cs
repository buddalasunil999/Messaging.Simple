using Newtonsoft.Json;

namespace Messaging.Simple
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly IMessageLogger messageLogger;
        private readonly ConnectionConfiguration connectionConfiguration;

        public MessageDispatcher(IMessageLogger messageLogger,
            ConnectionConfiguration connectionConfiguration)
        {
            this.messageLogger = messageLogger;
            this.connectionConfiguration = connectionConfiguration;
        }

        public void Send(string queue, string routingKey, string message)
        {
            using (var sender = new Sender(messageLogger, connectionConfiguration))
            {
                sender.Bind(queue, routingKey);
                sender.Send(routingKey, message);
            }
        }

        public void Send<T>(string queue, string routingKey, T obj)
        {
            Send(queue, routingKey, JsonConvert.SerializeObject(obj));
        }
    }
}