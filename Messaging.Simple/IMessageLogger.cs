using System;

namespace Messaging.Simple
{
    public interface IMessageLogger
    {
        void Info(string message);
        void Error(Exception exception);
    }
}