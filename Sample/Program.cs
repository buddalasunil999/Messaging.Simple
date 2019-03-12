using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Messaging.Simple;
using Messaging.Simple.Installers;

namespace Sample
{
    public class HelloMessageHandler : IMessageHandler
    {
        public void Handle(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class Test1MessageHandler : IMessageHandler
    {
        public void Handle(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class TestMessage
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Role { get; set; }

        public override string ToString()
        {
            return $"{Name} {Id} {Role}";
        }
    }

    public class Obj1MessageHandler : JsonMessageHandler<TestMessage>
    {
        public override void HandleData(TestMessage message)
        {
            Console.WriteLine(message);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MessageConfiguration[] configs = {
                new MessageConfiguration
                {
                    RoutingKey = "hello",
                    Queue = "hello",
                    Handler = typeof(HelloMessageHandler)
                },
                new MessageConfiguration
                {
                    RoutingKey = "test1",
                    Queue = "test1",
                    Handler = typeof(Test1MessageHandler)
                },
                new MessageConfiguration
                {
                    RoutingKey = "testobj1",
                    Queue = "testobj1",
                    Handler = typeof(Obj1MessageHandler)
                }
            };

            var container = new WindsorContainer();
            container.Install(new MessagingInstaller());
            foreach (var messageConfiguration in configs)
            {
                container.Register(
                    Component.For(messageConfiguration.Handler)
                        .Named(messageConfiguration.RoutingKey));
            }
            container.Register(
                Component.For<ConnectionConfiguration>()
                    .Instance(new ConnectionConfiguration
                    {
                        HostName = "localhost",
                        Exchange = "test"
                    }),
                Component.For<IMessageLogger>()
                    .ImplementedBy<ConsoleMessageLogger>()
                    .LifestyleSingleton());


            using (var receiver = container.Kernel.Resolve<Receiver>())
            {
                foreach (var messageConfiguration in configs)
                {
                    receiver.Run(messageConfiguration);
                }

                Console.ReadKey();
                container.Kernel.ReleaseComponent(receiver);
            }
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
