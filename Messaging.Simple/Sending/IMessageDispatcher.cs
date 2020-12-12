namespace Messaging.Simple
{
    public interface IMessageDispatcher
    {
        void Send(string routingKey, string message, string exchange);
        void Send(string routingKey, object obj, string exchange);
        void Send(string routingKey, object obj);
        void Send<T>(T obj, string exchange);
        void Send<T>(T obj);
    }
}