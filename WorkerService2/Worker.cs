using Messaging.Sample.Messages;
using Messaging.Simple;

namespace WorkerService2
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageDispatcher messageDispatcher;

        public Worker(ILogger<Worker> logger, IMessageDispatcher messageDispatcher)
        {
            _logger = logger;
            this.messageDispatcher = messageDispatcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int counter = 1;

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //await Task.Delay(1000, stoppingToken);

                messageDispatcher.Send(new TestMessage { Id = counter });
                messageDispatcher.Send(new TestMessage2 { Id = counter });
                counter++;
            }

            await Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}