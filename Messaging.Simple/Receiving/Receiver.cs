using System;
using System.Text;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using Newtonsoft.Json;
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

                messageDispatcher.Send("#", new PoisonMessage
                {
                    OriginalMessage = message,
                    Queue = config.Handler.FullName,
                    RoutingKey = e.RoutingKey
                },
                connectionConfiguration.PoisonExchange);
            });
        }

        public void RunPoison(string exchange, MessageConfiguration config)
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
                var message = Encoding.UTF8.GetString(e.Body);
                try
                {
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
                    if (!e.Redelivered)
                    {
                        Channel.BasicReject(deliveryTag: e.DeliveryTag, requeue: true);
                        messageLogger.Error(exception);
                    }
                    else
                    {
                        Channel.BasicReject(deliveryTag: e.DeliveryTag, requeue: false);
                        messageLogger.Error(exception);
                        onError(e);
                    }

                    throw;
                }
            };

            Channel.BasicConsume(queue: config.Handler.FullName,
                autoAck: false,
                consumer: consumer);
        }

        public bool RePublishUndeliveredMessage()
        {
            var result = Channel.BasicGet(connectionConfiguration.UndeliveredQueueName, true);
            if (result != null)
            {
                IBasicProperties props = result.BasicProperties;
                var message = Encoding.UTF8.GetString(result.Body);
                var poisonMessage = JsonConvert.DeserializeObject<PoisonMessage>(message);

                if (!string.IsNullOrEmpty(poisonMessage.Queue))
                {
                    Channel.BasicPublish(exchange: connectionConfiguration.Exchange,
                            routingKey: poisonMessage.Queue,
                            basicProperties: props,
                            body: Encoding.UTF8.GetBytes(poisonMessage.OriginalMessage));
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}