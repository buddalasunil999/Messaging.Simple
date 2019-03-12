using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging.Simple
{
    public class Receiver : Connection
    {
        private readonly IMessageLogger messageLogger;
        private readonly IHandlerFactory handlerFactory;

        public Receiver(IMessageLogger messageLogger, 
            ConnectionConfiguration connectionConfiguration,
            IHandlerFactory handlerFactory)
            : base(messageLogger, connectionConfiguration)
        {
            this.messageLogger = messageLogger;
            this.handlerFactory = handlerFactory;
        }

        public void Run(MessageConfiguration config)
        {
            Bind(config.Queue, config.RoutingKey);

            Channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (sender, e) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(e.Body);
                    messageLogger.Info($" [x] Received '{e.RoutingKey}':'{message}'");

                    var handler = handlerFactory.Resolve(e.RoutingKey);
                    handler.Handle(message);
                    handlerFactory.Release(handler);

                    Channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                }
                catch (Exception exception)
                {
                    messageLogger.Error(exception);
                    throw;
                }
            };

            Channel.BasicConsume(queue: config.Queue,
                autoAck: false,
                consumer: consumer);
        }
    }
}