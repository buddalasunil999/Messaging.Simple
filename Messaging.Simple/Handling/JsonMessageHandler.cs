using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Messaging.Simple
{
    public abstract class JsonMessageHandler<T> : IMessageHandler
    {
        public async Task HandleAsync(string message)
        {
            await HandleDataAsync(JsonConvert.DeserializeObject<T>(message));
        }

        public abstract Task HandleDataAsync(T message);
    }
}