using System.Threading.Tasks;

namespace Messaging.Simple
{
    public interface IMessageHandler
    {
        Task HandleAsync(string message);
    }
}