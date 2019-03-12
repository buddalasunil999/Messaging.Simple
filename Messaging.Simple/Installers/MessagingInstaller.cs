using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Messaging.Simple.Installers
{
    public class MessagingInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IHandlerFactory>()
                    .ImplementedBy<HandlerFactory>()
                    .LifestyleSingleton(),
                Component.For<IMessageDispatcher>()
                    .ImplementedBy<MessageDispatcher>()
                    .LifestyleSingleton(),
                Component.For<Receiver>()
                    .LifestyleSingleton());
        }
    }
}
