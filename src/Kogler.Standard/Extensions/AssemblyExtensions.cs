using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kogler.Standard
{
    public static class AssemblyExtensions
    {
        public static bool ContainsFullName(this IEnumerable<Assembly> collection, Assembly assembly)
        {
            return collection.Any(a => a.FullName == assembly.FullName);
        }
    }
}