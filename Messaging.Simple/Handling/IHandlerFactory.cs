using System;

namespace Messaging.Simple
{
    public interface IHandlerFactory
    {
        IMessageHandler Resolve(Type bindingKey);
    }
}