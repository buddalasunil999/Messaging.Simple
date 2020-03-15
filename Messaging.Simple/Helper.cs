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
            var assemblies = new List<Assembly>();
            var current = Assembly.GetEntryAssembly();
            assemblies.Add(current);

            assemblies.AddRange(current.GetReferencedAssemblies()
                .Where(x => x.Name.StartsWith(current.FullName.Split('.')[0]))
                .Select(x => Assembly.Load(x)));

            var types = new List<Type>();

            foreach(var assembly in assemblies)
            {
                var handlers = assembly?.GetTypes()
                .Where(y => y.BaseType != null &&
                y.BaseType.IsGenericType &&
                y.BaseType.GetGenericTypeDefinition() == typeof(JsonMessageHandler<>));

                if(handlers.Count() > 0)
                {
                    types.AddRange(handlers);
                }
            }

            return types;
        }
    }
}