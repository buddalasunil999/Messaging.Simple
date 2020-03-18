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

    class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorContainer();
            container.Install(new MessagingInstaller());
            container.Register(
                Component.For<ConnectionConfiguration>()
                    .Instance(new ConnectionConfiguration
                    {
                        HostName = "localhost",
                        Exchange = "test",
                        PoisionExchange = "poision",
                        PoisionQueueName = "poision-queue",
                        UndeliveredExchange = "undelivered",
                        UndeliveredQueueName = "undelivered-queue",
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
