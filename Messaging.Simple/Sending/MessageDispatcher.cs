using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

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
                sender.Send(routingKey, message, exchange);
            }
        }

        public void Send(string routingKey, object obj, string exchange)
        {
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    Send(routingKey, JsonConvert.SerializeObject(obj), exchange);
                    break;
                }
                catch (Exception)
                {
                    currentRetry++;

                    if (currentRetry > connectionConfiguration.SendRetryCount)
                    {
                        throw;
                    }
                }

                Task.Delay(TimeSpan.FromSeconds(connectionConfiguration.RetryWaitTimeInSeconds)).Wait();
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