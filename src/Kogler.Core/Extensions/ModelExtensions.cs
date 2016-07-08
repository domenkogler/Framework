using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kogler.Framework
{
    /// <summary>
    /// Extension methods for model entities.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Performs a shallow copy of all properties
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">Source entity object</param>
        /// <param name="dest">Destination entity object</param>
        public static void CopyValuesTo<T>(this T source, T dest)
        {
            var properties = typeof (T).GetTypeInfo().DeclaredProperties
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var property in properties)
            {
                if (property.SetMethod == null) continue;
                property.SetValue(dest, property.GetValue(source, null), null);
            }
        }

        /// <summary>
        /// Determines equality based on property hash codes.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">Source entity object</param>
        /// <param name="item">Object to compare</param>
        /// <param name="excludeProps">Properties excluded from comparison</param>
        /// <returns>True if object properties are the same</returns>
        public static bool AreSame<T>(this T source, T item, params string[] excludeProps)
            where T : class
        {
            int hashCode1 = GetObjectHashCode(source, excludeProps);
            int hashCode2 = GetObjectHashCode(item, excludeProps);
            return hashCode1 == hashCode2;
        }

        // Calculates object has code based on property hash codes
        private static int GetObjectHashCode(object item, params string[] excludeProps)
        {
            return
                item.GetType()
                    .GetTypeInfo()
                    .DeclaredProperties
                    .Where(prop => !excludeProps.Contains(prop.Name))
                    .Select(prop => prop.GetValue(item, null))
                    .Where(propVal => propVal != null)
                    .Aggregate(0, (current, propVal) => current | propVal.GetHashCode());
        }

        public static IEnumerable<FieldInfo> GetEnumFields(this Type enumType)
        {
            return enumType.GetTypeInfo().DeclaredFields.Where(f => f.IsLiteral && f.IsPublic && f.IsStatic);
        } 

        /// <summary>
        /// Convert an enum into a list of value / string pairs for 
        /// showing in list controls
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <returns>Sequence of enum / name pairs</returns>
        public static IEnumerable<EnumItem<TEnum>> GetEnumItems<TEnum>() where TEnum : struct
        {
            // Get TEnum type and verify it's an enum
            var enumType = typeof (TEnum);
            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException($"Type {enumType.Name} is not an enum");

            // Project enum values and names
            var items = from f in enumType.GetEnumFields()
                let val = (TEnum) f.GetValue(enumType)
                let name = f.Name
                select new EnumItem<TEnum>(val, name);
            return items;
        }

        /// <summary>
        /// Convert an enum into a list of value / string pairs for 
        /// showing in list controls
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <typeparam name="TValue">Enum base type</typeparam>
        /// <returns>Sequence of value / name pairs</returns>
        public static IEnumerable<EnumItem<TValue>> GetEnumItems<TEnum, TValue>()
            where TEnum : struct
            where TValue : struct
        {
            // Get TEnum type and verify it's an enum
            var enumType = typeof (TEnum);
            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException($"Type {enumType.Name} is not an enum");

            // Verify TValue is an allowed numeric type
            var valType = typeof (TValue);
            if (!IsValidEnumBase(valType))
                throw new ArgumentException($"Type {enumType.Name} is not a valid enum base type");

            // Project enum values and names
            var items = from f in enumType.GetEnumFields()
                let val = (TValue) f.GetValue(enumType)
                let name = f.Name
                select new EnumItem<TValue>(val, name);
            return items;
        }

        private static bool IsValidEnumBase(Type valType)
        {
            return valType == typeof (byte)
                   || valType == typeof (short)
                   || valType == typeof (int)
                   || valType == typeof (long);
        }

        public class EnumItem<TValue>
        {
            public EnumItem()
            {
            }

            public EnumItem(TValue value, string name)
            {
                Value = value;
                Name = name;
            }

            public TValue Value { get; private set; }
            public string Name { get; private set; }
        }
    }
}