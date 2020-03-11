using System;

namespace Messaging.Simple
{
    public class MessageConfiguration
    {
        private string routingKey;

        public string RoutingKey
        {
            get => string.IsNullOrEmpty(routingKey) ? Handler.BaseType?.GenericTypeArguments[0].FullName : routingKey;
            set => routingKey = value;
        }

        public Type Handler { get; set; }
    }
}