using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Messaging.Sample.Messages;
using Messaging.Simple;
using Messaging.Simple.Installers;

namespace Messaging.Sample.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var container = new WindsorContainer();
            container.Install(new MessagingInstaller());
            container.Register(
                Component.For<RabbitMqConfiguration>()
                    .Instance(new RabbitMqConfiguration
                    {
                        HostName = "localhost",
                        Exchange = "sample.test",
                        PoisionExchange = "sample.poision",
                        DelayedExchange = "sample.delayed",
                        PoisionQueueName = "sample.poision-queue",
                        UndeliveredExchange = "sample.undelivered",
                        UndeliveredQueueName = "sample.undelivered-queue",
                        UserName = "guest",
                        Password = "guest"
                    }),
                Component.For<IMessageLogger>()
                    .ImplementedBy<ConsoleMessageLogger>()
                    .LifestyleSingleton());

            var dispatcher = container.Kernel.Resolve<IMessageDispatcher>();

            dispatcher.Send(new TestMessage { Id = 2 });

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
            Console.WriteLine(message);
        }

        public void Error(Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}
