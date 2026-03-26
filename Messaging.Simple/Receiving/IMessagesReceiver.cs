using System.Threading;

namespace Messaging.Simple
{
    public interface IMessagesReceiver
    {
        void Run(CancellationToken stoppingToken);
        void ProcessUndeliveredMessages();
    }
}
