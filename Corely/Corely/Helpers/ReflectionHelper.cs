using System;
using System.Linq;
using System.Reflection;

namespace Corely.Helpers
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Use reflection to return a property from any object type
        /// </summary>
        /// <param name="propname"></param>
        /// <returns></returns>
        public static string GetResource<T>(string propname)
        {
            return GetResource(typeof(T), propname);
        }

        /// <summary>
        /// Use reflection to return a property from given type
        /// </summary>
        /// <param name="t"></param>
        /// <param name="propname"></param>
        /// <returns></returns>
        public static string GetResource(Type t, string propname)
        {
            PropertyInfo[] props = t.GetProperties();
            return props.FirstOrDefault(m => m.Name == propname)?.GetValue(propname)?.ToString() ?? propname;
        }

    }
}
