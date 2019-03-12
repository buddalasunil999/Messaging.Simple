namespace Messaging.Simple
{
    public interface IMessageHandler
    {
        void Handle(string message);
    }
}