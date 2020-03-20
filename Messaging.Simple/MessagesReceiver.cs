namespace Messaging.Simple
{
    public class MessagesReceiver : IMessagesReceiver
    {
        protected readonly Receiver receiver;
        protected readonly IMessageLogger messageLogger;
        protected readonly ConnectionConfiguration connectionConfiguration;

        public MessagesReceiver(Receiver receiver,
            IMessageLogger messageLogger,
            ConnectionConfiguration connectionConfiguration)
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
                receiver.Run(connectionConfiguration.Exchange,
                    new MessageConfiguration
                    {
                        Handler = handler
                    });
                messageLogger.Info($"Receiver for {handler}");
            }
        }
    }
}