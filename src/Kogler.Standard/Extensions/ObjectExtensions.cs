using System;
using System.Reflection;

namespace Kogler.Standard
{
    public static class ObjectExtensions
    {
        public static bool IsNullable<T>(this T obj)
        {
            if (obj == null) return true; // obvious
            Type type = typeof(T);
            if (!type.GetTypeInfo().IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }
    }
}