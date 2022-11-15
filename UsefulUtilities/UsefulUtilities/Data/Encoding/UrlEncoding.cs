using System;

namespace UsefulUtilities.Data.Encoding
{
    public static class UrlEncoding
    {
        /// <summary>
        /// Encode string for URL
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string UrlEncode(this string source)
        {
            if(source == null) { return null; }
            return Uri.EscapeDataString(source);
        }

        /// <summary>
        /// Decode URL encoded string
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string UrlDecode(this string source)
        {
            if(source == null) { return null; }
            source = source.Replace("+", "%20");
            return Uri.UnescapeDataString(source);
        }

    }
}
