using Messaging.Sample.Messages;
using Messaging.Simple;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessagesReceiver messagesReceiver;

        public Worker(ILogger<Worker> logger, IMessagesReceiver messagesReceiver)
        {
            _logger = logger;
            this.messagesReceiver = messagesReceiver;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            messagesReceiver.Run(stoppingToken);

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    await Task.Delay(1000, stoppingToken);
            //}

            await Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }

    public class ConsoleMessageLogger : IMessageLogger
    {
        public List<string> Messages { set; get; }

        public ConsoleMessageLogger()
        {
            Messages = new List<string>();
        }

        public void Info(string message)
        {
            Messages.Add(message);
        }

        public void Error(Exception exception)
        {
            Messages.Add(exception.Message);
        }

        public void Undelivered(string routingKey, string obj, string exchange)
        {
            throw new NotImplementedException();
        }
    }

    public class Obj1MessageHandler : JsonMessageHandler<TestMessage>
    {
        public override async Task HandleDataAsync(TestMessage message)
        {
            await Task.Delay(5000);
            Console.WriteLine("from first handler" + message);
        }
    }

    public class Obj1MessageHandler2 : JsonMessageHandler<TestMessage>
    {
        public override  async Task HandleDataAsync(TestMessage message)
        {
            await Task.Delay(10000);
            Console.WriteLine("from second handler" + message);
        }
    }

    public class Obj1MessageHandler3 : JsonMessageHandler<TestMessage>
    {
        public override Task HandleDataAsync(TestMessage message)
        {
            Console.WriteLine("from third handler" + message);
            throw new Exception("failed for third");
        }
    }

    public class TestPoisonMessageHandler : PoisonMessageHandler
    {
        public override Task HandleDataAsync(PoisonMessage message)
        {
            Console.WriteLine($"from TestPoisonMessageHandler: {message.OriginalMessage}, {message.Queue}");
            throw new Exception("failed for poison");
        }
    }
}