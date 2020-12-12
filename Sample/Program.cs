using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Messaging.Sample.Messages;
using Messaging.Simple;
using Messaging.Simple.Installers;

namespace Messaging.Sample.Receiver
{
    public class Obj1MessageHandler : JsonMessageHandler<TestMessage>
    {
        public override Task HandleDataAsync(TestMessage message)
        {
            Console.WriteLine("from first handler" + message);
            return Task.CompletedTask;
        }
    }

    public class Obj1MessageHandler2 : JsonMessageHandler<TestMessage>
    {
        public override Task HandleDataAsync(TestMessage message)
        {
            Console.WriteLine("from second handler" + message);
            return Task.CompletedTask;
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

    class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorContainer();
            container.Install(new MessagingInstaller());
            container.Register(
                Component.For<RabbitMqConfiguration>()
                    .Instance(new RabbitMqConfiguration
                    {
                        HostName = "localhost",
                        Exchange = "sample.test",
                        PoisonExchange = "sample.poison",
                        DelayedExchange = "sample.delayed",
                        UndeliveredExchange = "sample.undelivered",
                        UndeliveredQueueName = "sample.undelivered-queue",
                        UserName = "guest",
                        Password = "guest"
                    }),
                Component.For<IMessageLogger>()
                    .ImplementedBy<ConsoleMessageLogger>()
                    .LifestyleSingleton());

            var receiver = container.Kernel.Resolve<IMessagesReceiver>();
            receiver.Run();

            Console.ReadKey();
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
    }
}
