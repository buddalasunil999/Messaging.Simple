using Newtonsoft.Json;

namespace Messaging.Simple
{
    public abstract class JsonMessageHandler<T> : IMessageHandler
    {
        public void Handle(string message)
        {
            HandleData(JsonConvert.DeserializeObject<T>(message));
        }

        public abstract void HandleData(T message);
    }
}