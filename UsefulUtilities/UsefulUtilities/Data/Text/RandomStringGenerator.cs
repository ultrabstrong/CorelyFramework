using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Data.Text
{
    public static class RandomStringGenerator
    {
        /// <summary>
        /// Create new random number generator
        /// </summary>
        private static Random Random { get; set; } = new Random();

        /// <summary>
        /// Generate random string
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
