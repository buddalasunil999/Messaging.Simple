namespace Messaging.Simple
{
    public class MessagesReceiver : IMessagesReceiver
    {
        private readonly Receiver receiver;

        public MessagesReceiver(Receiver receiver)
        {
            this.receiver = receiver;
        }

        public void Run()
        {
            foreach (var handler in Helper.GetAllHandlers())
            {
                receiver.Run(new MessageConfiguration
                {
                    Handler = handler
                });
            }
        }
    }
}