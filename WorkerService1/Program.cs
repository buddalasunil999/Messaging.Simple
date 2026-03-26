using Messaging.Simple;
using WorkerService1;

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
