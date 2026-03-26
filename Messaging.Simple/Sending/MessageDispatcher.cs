using Newtonsoft.Json;
using System;

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

        protected void Send(string routingKey, string message, string exchange)
        {
            using (var sender = new Sender(messageLogger, connectionConfiguration))
            {
                sender.Init();
                sender.Send(routingKey, message, exchange);
            }
        }

        public void Send(string routingKey, object obj, string exchange)
        {
            try
            {
                Send(routingKey, JsonConvert.SerializeObject(obj), exchange);
            }
            catch (Exception ex)
            {
                messageLogger.Error(ex);
                messageLogger.Undelivered(routingKey, JsonConvert.SerializeObject(obj), exchange);
            }
        }

        public void Send(string routingKey, string obj)
        {
            Send(routingKey, obj, connectionConfiguration.Exchange);
        }

        public void Send<T>(T obj)
        {
            Send(typeof(T).ToString(), obj, connectionConfiguration.Exchange);
        }
    }
}