namespace Messaging.Simple
{
    public interface IMessageDispatcher
    {
        void Send(string queue, string routingKey, string message);
        void Send<T>(string queue, string routingKey, T obj);
    }
}