namespace UsefulUtilities.Extensions
{
    public static class ReflectionExtension
    {
        /// <summary>
        /// Use reflection to return a property from any object type
        /// </summary>
        /// <param name="propname"></param>
        /// <returns></returns>
        public static string GetResource<T>(this T type, string propname)
        {
            return Helpers.ReflectionHelper.GetResource<T>(propname);
        }
    }
}
