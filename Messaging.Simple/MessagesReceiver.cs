namespace Messaging.Simple
{
    public class MessagesReceiver : IMessagesReceiver
    {
        private readonly Receiver receiver;
        private readonly IMessageLogger messageLogger;

        public MessagesReceiver(Receiver receiver, IMessageLogger messageLogger)
        {
            this.receiver = receiver;
            this.messageLogger = messageLogger;
        }

        public void Run()
        {
            messageLogger.Info($"Starting {typeof(MessagesReceiver)}");
            foreach (var handler in Helper.GetAllHandlers())
            {
                receiver.Run(new MessageConfiguration
                {
                    Handler = handler
                });
                messageLogger.Info($"Receiver for {handler}");
            }
        }
    }
}