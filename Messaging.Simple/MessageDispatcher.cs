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

        public void Send(string routingKey, string message)
        {
            using (var sender = new Sender(messageLogger, connectionConfiguration))
            {
                sender.Send(routingKey, message);
            }
        }

        public void Send<T>(string routingKey, T obj)
        {
            Send(routingKey, JsonConvert.SerializeObject(obj));
        }

        public void Send<T>(T obj)
        {
            Send(typeof(T).ToString(), obj);
        }
    }
}