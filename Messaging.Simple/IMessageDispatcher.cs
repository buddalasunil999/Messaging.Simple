namespace Messaging.Simple
{
    public interface IMessageDispatcher
    {
        void Send(string routingKey, string message);
        void Send<T>(string routingKey, T obj);
        void Send<T>(T obj);
    }
}