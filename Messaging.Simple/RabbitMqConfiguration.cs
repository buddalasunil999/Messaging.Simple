namespace Messaging.Simple
{
    public class RabbitMqConfiguration
    {
        public string HostName { get; set; }
        public string Exchange { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string PoisionExchange { get; set; }
        public string PoisionQueueName { get; set; }
        public string UndeliveredExchange { get; set; }
        public string UndeliveredQueueName { get; set; }
        public string DelayedExchange { get; set; }
    }
}