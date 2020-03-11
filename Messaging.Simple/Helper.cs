using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Messaging.Simple
{
    public static class Helper
    {
        public static IEnumerable<Type> GetAllHandlers()
        {
            return Assembly.GetEntryAssembly()
                ?.GetTypes()
                .Where(x => x.BaseType.IsGenericType && x.BaseType.GetGenericTypeDefinition() == typeof(JsonMessageHandler<>));
        }
    }
}