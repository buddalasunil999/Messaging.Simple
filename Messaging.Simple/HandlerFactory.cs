using Castle.MicroKernel;

namespace Messaging.Simple
{
    public class HandlerFactory : IHandlerFactory
    {
        private readonly IKernel kernel;

        public HandlerFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public IMessageHandler Resolve(string routingKey)
        {
            return kernel.Resolve<IMessageHandler>(routingKey);
        }

        public void Release(IMessageHandler messageHandler)
        {
            kernel.ReleaseComponent(messageHandler);
        }
    }
}