namespace Messaging.Simple
{
    public interface IMessageDispatcher
    {
        void Send(string routingKey, object obj, string exchange);
        void Send(string routingKey, string obj);
        void Send<T>(T obj);
    }
}