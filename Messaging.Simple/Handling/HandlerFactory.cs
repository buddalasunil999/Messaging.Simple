using System;

namespace Messaging.Simple
{
    public class HandlerFactory : IHandlerFactory
    {
        private readonly IServiceProvider serviceProvider;

        public HandlerFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IMessageHandler Resolve(Type bindingKey)
        {
            return (IMessageHandler)serviceProvider.GetService(bindingKey);
        }
    }
}