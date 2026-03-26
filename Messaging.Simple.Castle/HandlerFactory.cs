using Castle.MicroKernel;
using System;

namespace Messaging.Simple.Castle
{
    public class HandlerFactory : IHandlerFactory
    {
        private readonly IKernel kernel;

        public HandlerFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public IMessageHandler Resolve(Type bindingKey)
        {
            return kernel.Resolve<IMessageHandler>(bindingKey.FullName);
        }

        public void Release(IMessageHandler messageHandler)
        {
            kernel.ReleaseComponent(messageHandler);
        }
    }
}