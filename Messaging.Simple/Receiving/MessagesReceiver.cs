﻿namespace Messaging.Simple
{
    public class MessagesReceiver : IMessagesReceiver
    {
        protected readonly Receiver receiver;
        protected readonly IMessageLogger messageLogger;
        protected readonly RabbitMqConfiguration connectionConfiguration;

        public MessagesReceiver(Receiver receiver,
            IMessageLogger messageLogger,
            RabbitMqConfiguration connectionConfiguration)
        {
            this.receiver = receiver;
            this.messageLogger = messageLogger;
            this.connectionConfiguration = connectionConfiguration;
        }

        public virtual void Run()
        {
            messageLogger.Info($"Starting {typeof(MessagesReceiver)}");
            foreach (var handler in Helper.GetAllHandlers())
            {
                if (typeof(PoisionMessageHandler).IsAssignableFrom(handler))
                {
                    receiver.RunPoision(connectionConfiguration.PoisionExchange,
                        new MessageConfiguration
                    {
                        RoutingKey = "#",
                        Handler = handler
                    });
                }
                else
                {
                    receiver.Run(connectionConfiguration.Exchange,
                        new MessageConfiguration
                        {
                            Handler = handler
                        });
                }
                messageLogger.Info($"Receiver for {handler}");
            }
        }
    }
}