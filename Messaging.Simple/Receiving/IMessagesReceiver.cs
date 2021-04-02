namespace Messaging.Simple
{
    public interface IMessagesReceiver
    {
        void Run();
        void ProcessUndeliveredMessages();
    }
}
