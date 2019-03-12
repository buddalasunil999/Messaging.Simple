using System;

namespace Messaging.Simple
{
    public class MessageConfiguration
    {
        public string Queue { get; set; }
        public string RoutingKey { get; set; }
        public Type Handler { get; set; }
    }
}