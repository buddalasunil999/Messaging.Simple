using System;
using RabbitMQ.Client;

namespace Messaging.Simple
{
    public abstract class Connection : IDisposable
    {
        private readonly IMessageLogger messageLogger;
        private readonly ConnectionConfiguration config;
        private readonly IConnection connection;
        protected readonly IModel Channel;

        protected Connection(IMessageLogger messageLogger,
            ConnectionConfiguration configuration)
        {
            this.messageLogger = messageLogger;
            config = configuration;

            var factory = new ConnectionFactory
            {
                HostName = config.HostName,
                AutomaticRecoveryEnabled = true
            };
            connection = factory.CreateConnection();
            Channel = connection.CreateModel();
            Channel.ExchangeDeclare(exchange: config.Exchange, type: "direct");
        }

        public void Bind(string queue, string routingKey)
        {
            Channel.QueueDeclare(queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            Channel.QueueBind(queue: queue,
                exchange: config.Exchange,
                routingKey: routingKey);
        }

        public void Dispose()
        {
            Channel?.Close();
            connection?.Close();
            Channel?.Dispose();
            connection?.Dispose();
            messageLogger.Info($"Disconnected from RabbitMQ {config.HostName}");
        }
    }
}