using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kogler.Standard
{
    public static class TypeExtensions
    {
        public static bool HasInterface(this Type type, Type interfaceType)
        {
            return interfaceType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        public static bool HasInterface<TInterface>(this Type type)
        {
            return type.HasInterface(typeof(TInterface));
        }

        public static bool IsGenericSubclassOf(this Type toCheck, Type generic)
        {
            return GetGenericSubclassOf(toCheck, generic) != null;
        }

        public static Type GetGenericSubclassOf(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.GetTypeInfo().IsGenericType ? toCheck.GetTypeInfo().GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return toCheck;
                }
                toCheck = toCheck.GetTypeInfo().BaseType;
            }
            return null;
        }

        /// <summary>
        ///   Gets the attributes.
        /// </summary>
        /// <param name = "attributes">The attributes.</param>
        /// <returns>The attributes.</returns>
        public static IEnumerable<T> GetAttributes<T>(this IEnumerable<CustomAttributeData> attributes) where T : class
        {
            return attributes
                .Select(d => d.AttributeType)
                .Where(t => t.GetTypeInfo().IsAssignableFrom(typeof (T).GetTypeInfo())).OfType<T>();
        }
    }
}