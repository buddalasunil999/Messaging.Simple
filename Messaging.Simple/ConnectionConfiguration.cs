namespace Messaging.Simple
{
    public class ConnectionConfiguration
    {
        public string HostName { get; set; }
        public string Exchange { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Protocol { get; set; }

        public string PoisionExchange { get; set; }
        public string PoisionQueueName { get; set; }
        public string UndeliveredExchange { get; set; }
        public string UndeliveredQueueName { get; set; }

        public string Uri => $"{Protocol}://{UserName}:{Password}@{HostName}:{Port}";
    }
}