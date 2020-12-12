namespace Messaging.Simple
{
    public class PoisionMessage
    {
        public string OriginalMessage { get; set; }
        public string Queue { get; set; }
        public string RoutingKey { get; set; }
    }
}