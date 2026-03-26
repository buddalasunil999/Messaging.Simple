using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging.Simple
{
    public class Receiver : Connection
    {
        private readonly IMessageLogger messageLogger;
        private readonly IHandlerFactory handlerFactory;
        private readonly IMessageDispatcher messageDispatcher;

        public Receiver(IMessageLogger messageLogger,
            RabbitMqConfiguration connectionConfiguration,
            IHandlerFactory handlerFactory,
            IMessageDispatcher messageDispatcher)
            : base(messageLogger, connectionConfiguration)
        {
            this.messageLogger = messageLogger;
            this.handlerFactory = handlerFactory;
            this.messageDispatcher = messageDispatcher;
        }

        public void Run(string exchange, MessageConfiguration config, CancellationToken cancellationToken)
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
            }, cancellationToken);
        }

        public void RunPoison(string exchange, MessageConfiguration config, CancellationToken cancellationToken)
        {
            Run(exchange, config, e =>
            {
                Channel.BasicPublish(exchange: connectionConfiguration.UndeliveredExchange,
                        routingKey: e.RoutingKey,
                        basicProperties: e.BasicProperties,
                        body: e.Body);
            }, cancellationToken);
        }

        private void Run(string exchange, MessageConfiguration config, Action<BasicDeliverEventArgs> onError, CancellationToken cancellationToken)
        {
            Bind(config.Handler.FullName, config.RoutingKey, exchange);

            Channel.BasicQos(prefetchSize: 0, prefetchCount: connectionConfiguration.PrefetchCount, global: false);

            var consumer = new AsyncEventingBasicConsumer(Channel);
            consumer.Received += (sender, e) =>
            {
                var args = e;
                var message = Encoding.UTF8.GetString(args.Body);

                messageLogger.Info($" [x] Received '{args.RoutingKey}':'{message}'");

                Task.Run(async () =>
                {
                    // Check for cancellation requests
                    cancellationToken.ThrowIfCancellationRequested();

                    await OnReceived(config, onError, args, message);
                }, cancellationToken);

                return Task.CompletedTask;
            };

            Channel.BasicConsume(queue: config.Handler.FullName,
            autoAck: false,
            consumer: consumer);
        }

        private async Task OnReceived(MessageConfiguration config, 
            Action<BasicDeliverEventArgs> onError,
            BasicDeliverEventArgs args,
            string message)
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var handler = handlerFactory.Resolve(config.Handler);
                await handler.HandleAsync(message);

                stopwatch.Stop();
                messageLogger.Info($"Finished {config.Handler}. Execution time {stopwatch.ElapsedMilliseconds} ms");

                Channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
            }
            catch (Exception exception)
            {
                if (!args.Redelivered)
                {
                    Channel.BasicReject(deliveryTag: args.DeliveryTag, requeue: true);
                    messageLogger.Error(exception);
                }
                else
                {
                    Channel.BasicReject(deliveryTag: args.DeliveryTag, requeue: false);
                    messageLogger.Error(exception);
                    onError(args);
                }

                throw;
            }
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