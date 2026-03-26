//using System;
//using System.Collections.Generic;
//using Xunit;

//namespace Messaging.Simple.Tests
//{
//    public class UnitTest1
//    {
//        private readonly ConnectionConfiguration connection = new ConnectionConfiguration
//        {
//            HostName = "localhost",
//            Exchange = "test"
//        };

//        [Fact]
//        public void SendTestHello()
//        {
//            var config = new MessageConfiguration
//            {
//                RoutingKey = "hello",
//                Queue = "hello"
//            };
//            var logger = new ConsoleMessageLogger();

//            var dispatcher = new MessageDispatcher(logger, connection);
//            dispatcher.Send(config.Queue, config.RoutingKey, "Hello world");

//            Assert.Contains("Hello world", logger.Messages[0]);
//        }

//        [Fact]
//        public void SendTest1()
//        {
//            var config = new MessageConfiguration
//            {
//                RoutingKey = "test1",
//                Queue = "test1"
//            };
//            var logger = new ConsoleMessageLogger();

//            var dispatcher = new MessageDispatcher(logger, connection);
//            dispatcher.Send(config.Queue, config.RoutingKey, "test1 world");

//            Assert.Contains("test1 world", logger.Messages[0]);
//        }

//        [Fact]
//        public void SendObject1()
//        {
//            var config = new MessageConfiguration
//            {
//                RoutingKey = "testobj1",
//                Queue = "testobj1"
//            };
//            var logger = new ConsoleMessageLogger();

//            var dispatcher = new MessageDispatcher(logger, connection);
//            dispatcher.Send(config.Queue,
//                config.RoutingKey,
//                new
//                {
//                    Name = "test",
//                    Id = 1,
//                    Role = "Sendxxxx"
//                });

//            Assert.Contains("Sendxxxx", logger.Messages[0]);
//        }
//    }

//    public class ConsoleMessageLogger : IMessageLogger
//    {
//        public List<string> Messages { set; get; }

//        public ConsoleMessageLogger()
//        {
//            Messages = new List<string>();
//        }

//        public void Info(string message)
//        {
//            Messages.Add(message);
//        }

//        public void Error(Exception exception)
//        {
//            Messages.Add(exception.Message);
//        }
//    }
//}
