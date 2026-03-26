using Messaging.Simple;
using WorkerService2;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddMessaging();
        services.AddSingleton<IMessageLogger, ConsoleMessageLogger>();
        services.AddSingleton(new RabbitMqConfiguration
        {
            HostName = "localhost",
            Exchange = "sample.test",
            PoisonExchange = "sample.poison",
            DelayedExchange = "sample.delayed",
            UndeliveredExchange = "sample.undelivered",
            UndeliveredQueueName = "sample.undelivered-queue",
            UserName = "guest",
            Password = "guest"
        });
    })
    .Build();

await host.RunAsync();

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