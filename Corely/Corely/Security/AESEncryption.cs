using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Corely.Security
{
    public static class AESEncryption
    {
        /// <summary>
        /// Encrypt an unprotected value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="unprotectedValue"></param>
        /// <returns></returns>
        public static string Encrypt(string key, string unprotectedValue)
        {
            // Get unprotected value bytes
            byte[] unprotectedValueBytes = Encoding.ASCII.GetBytes(unprotectedValue.ToCharArray());
            // Pad key to 256 bits
            if (key.Length >= 32) { key = key.Substring(0, 32); }
            else { key = key.PadRight(32, char.Parse("X")); }
            byte[] keyBytes = Encoding.ASCII.GetBytes(key.ToCharArray());
            // Create IV
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] iv = new byte[16];
            provider.GetBytes(iv);
            // Initialize byte array and streams for encrypting
            byte[] protectedValueBytes = null;
            using (RijndaelManaged rm = new RijndaelManaged())
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, rm.CreateEncryptor(keyBytes, iv), CryptoStreamMode.Write))
            {
                // Encrypt unprotected value with key and IV
                cs.Write(unprotectedValueBytes, 0, unprotectedValueBytes.Length);
                cs.FlushFinalBlock();
                protectedValueBytes = iv.Concat(ms.ToArray()).ToArray();
            }
            // Return base64 protected value
            return Convert.ToBase64String(protectedValueBytes);
        }

        /// <summary>
        /// Decrypt a protected value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="protectedValue"></param>
        /// <returns></returns>
        public static string Decrypt(string key, string protectedValue)
        {
            // Decode base64 protected value
            byte[] protectedValueBytes = Convert.FromBase64String(protectedValue);
            // Pad key to is 256 bits
            if (key.Length >= 32) { key = key.Substring(0, 32); }
            else { key = key.PadRight(32, char.Parse("X")); }
            byte[] keyBytes = Encoding.ASCII.GetBytes(key.ToCharArray());
            // Remove random IV from beginning of value
            byte[] iv = new byte[16];
            Array.Copy(protectedValueBytes, 0, iv, 0, 16);
            protectedValueBytes = protectedValueBytes.Skip(16).ToArray();
            // Initialize byte array and streams for decrypting
            byte[] unprotectedValue = new byte[protectedValueBytes.Length];
            using (RijndaelManaged rm = new RijndaelManaged())
            using (MemoryStream ms = new MemoryStream(protectedValueBytes))
            using (CryptoStream cs = new CryptoStream(ms, rm.CreateDecryptor(keyBytes, iv), CryptoStreamMode.Read))
            {
                // Decrypt protected value with key and IV
                cs.Read(unprotectedValue, 0, unprotectedValue.Length);
            }
            // Clear control chars
            List<char> chars = Encoding.ASCII.GetChars(unprotectedValue).ToList();
            for (int i = chars.Count - 1; i > -1; i--)
            {
                if (Char.IsControl(chars[i]))
                {
                    chars.RemoveAt(i);
                }
            }
            // Convert chars to string
            return new string(chars.ToArray());
        }

        /// <summary>
        /// Create and return a random base64 key
        /// </summary>
        /// <returns></returns>
        public static string CreateRandomBase64Key()
        {
            string base64string = "";
            using (RijndaelManaged objRM = new RijndaelManaged())
            {
                objRM.GenerateKey();
                base64string = Convert.ToBase64String(objRM.Key);
            }
            return base64string;
        }
    }
}
