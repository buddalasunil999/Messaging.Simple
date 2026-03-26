using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Messaging.Simple
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMessageDispatcher, MessageDispatcher>();
            serviceCollection.AddTransient<Receiver>();
            serviceCollection.AddTransient<IMessagesReceiver, MessagesReceiver>();
            serviceCollection.AddSingleton<IHandlerFactory, HandlerFactory>();

            foreach (var type in Helper.GetAllHandlers())
            {
                serviceCollection.AddTransient(type);
            }

            return serviceCollection;
        }

        public static IServiceCollection AddMessaging(this IServiceCollection serviceCollection, params Assembly[] handlerAssemblies)
        {
            serviceCollection.AddTransient<IMessageDispatcher, MessageDispatcher>();
            serviceCollection.AddTransient<Receiver>();
            serviceCollection.AddTransient<IMessagesReceiver, MessagesReceiver>();
            serviceCollection.AddSingleton<IHandlerFactory, HandlerFactory>();

            foreach (var type in Helper.GetAllHandlers(handlerAssemblies))
            {
                serviceCollection.AddTransient(type);
            }

            return serviceCollection;
        }
    }
}
