using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Extensions
{
    public static class HashAlgorithmTypeExtension
    {
        /// <summary>
        /// Get hash algorithm name for hash algorithm type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static HashAlgorithmName? GetAlgorithm(this HashAlgorithmType type)
        {
            switch (type)
            {
                case HashAlgorithmType.Sha1:
                    return HashAlgorithmName.SHA1;
                case HashAlgorithmType.Sha256:
                    return HashAlgorithmName.SHA256;
                case HashAlgorithmType.Sha384:
                    return HashAlgorithmName.SHA384;
                case HashAlgorithmType.Sha512:
                    return HashAlgorithmName.SHA512;
                case HashAlgorithmType.Md5:
                    return HashAlgorithmName.MD5;
                case HashAlgorithmType.None:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get hash algorithm name for hash algorithm type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static HashAlgorithmName GetAlgorithmOrDefault(this HashAlgorithmType type)
        {
            return type.GetAlgorithm() ?? HashAlgorithmName.SHA256;
        }

        /// <summary>
        /// Get hash algorithm string name for hash algorithm type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetAlgorithmName(this HashAlgorithmType type)
        {
            return type.GetAlgorithm()?.Name;
        }
    }
}
