using System.IO;
using System.Runtime.Serialization;

namespace Kogler.Framework
{
    /// <summary>
    /// Extension methods for model entities.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Performs a deep copy using DatacontractSerializer.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="obj">Entity object</param>
        /// <returns>Cloned entity</returns>
        public static T Clone<T>(this T obj)
        {
            T copy;
            using (MemoryStream stream = new MemoryStream())
            {
                var ser = new DataContractSerializer(typeof (T));
                ser.WriteObject(stream, obj);
                stream.Position = 0;
                copy = (T) ser.ReadObject(stream);
            }
            return copy;
        }
    }
}