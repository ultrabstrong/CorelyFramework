using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Data.Encoding
{
    public static class Base64String
    {
        /// <summary>
        /// Return base64 encoded string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Base64Encode(string s)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(s));
        }

        /// <summary>
        /// Return base64 decoded string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Base64Decode(string s)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(s));
        }
    }
}
