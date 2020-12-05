using System;
using System.Collections.Generic;
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

        public Receiver(IMessageLogger messageLogger, 
            RabbitMqConfiguration connectionConfiguration,
            IHandlerFactory handlerFactory,
            IKernel kernel)
            : base(messageLogger, connectionConfiguration)
        {
            this.messageLogger = messageLogger;
            this.handlerFactory = handlerFactory;
            this.kernel = kernel;
        }

        public void Run(string exchange, MessageConfiguration config)
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

                    if (e.BasicProperties.Headers == null)
                    {
                        e.BasicProperties.Headers = new Dictionary<string, object>();
                    }
                    e.BasicProperties.Headers.Add("Queue", config.Handler.FullName);

                    Channel.BasicPublish(exchange: connectionConfiguration.PoisionExchange,
                        routingKey: config.RoutingKey,
                        basicProperties: e.BasicProperties,
                        body: e.Body);

                    messageLogger.Error(exception);
                    throw;
                }
            };

            Channel.BasicConsume(queue: config.Handler.FullName,
                autoAck: false,
                consumer: consumer);
        }
    }
}