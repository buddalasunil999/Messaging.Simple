using System;
using System.Text;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging.Simple
{
    public class Receiver : Connection
    {
        private readonly IMessageLogger messageLogger;
        private readonly IHandlerFactory handlerFactory;
        private readonly IKernel kernel;
        private readonly IMessageDispatcher messageDispatcher;

        public Receiver(IMessageLogger messageLogger,
            RabbitMqConfiguration connectionConfiguration,
            IHandlerFactory handlerFactory,
            IKernel kernel,
            IMessageDispatcher messageDispatcher)
            : base(messageLogger, connectionConfiguration)
        {
            this.messageLogger = messageLogger;
            this.handlerFactory = handlerFactory;
            this.kernel = kernel;
            this.messageDispatcher = messageDispatcher;
        }

        public void Run(string exchange, MessageConfiguration config)
        {
            Run(exchange, config, e =>
            {
                var message = Encoding.UTF8.GetString(e.Body);

                messageDispatcher.Send(new PoisionMessage
                {
                    OriginalMessage = message,
                    Queue = config.Handler.FullName,
                    RoutingKey = e.RoutingKey
                },
                connectionConfiguration.PoisionExchange);
            });
        }

        public void RunPoision(string exchange, MessageConfiguration config)
        {
            Run(exchange, config, e =>
            {
                Channel.BasicPublish(exchange: connectionConfiguration.UndeliveredExchange,
                        routingKey: e.RoutingKey,
                        basicProperties: e.BasicProperties,
                        body: e.Body);
            });
        }

        private void Run(string exchange, MessageConfiguration config, Action<BasicDeliverEventArgs> onError)
        {
            Bind(config.Handler.FullName, config.RoutingKey, exchange);

            Channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new AsyncEventingBasicConsumer(Channel);
            consumer.Received += async (sender, e) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(e.Body);
                    messageLogger.Info($" [x] Received '{e.RoutingKey}':'{message}'");

                    using (kernel.BeginScope())
                    {
                        var handler = handlerFactory.Resolve(config.Handler.FullName);
                        await handler.HandleAsync(message);
                        handlerFactory.Release(handler);
                    }

                    Channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                }
                catch (Exception exception)
                {
                    Channel.BasicReject(deliveryTag: e.DeliveryTag, requeue: false);
                    messageLogger.Error(exception);
                    onError(e);
                    throw;
                }
            };

            Channel.BasicConsume(queue: config.Handler.FullName,
                autoAck: false,
                consumer: consumer);
        }
    }
}