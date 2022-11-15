using Corely.Data.Encoding;
using Corely.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Corely.Security
{
    [Serializable]
    public class HMACHashedValue : HashedValue
    {
        #region Constructors

        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public HMACHashedValue() { }

        /// <summary>
        /// Construct with value to hash
        /// </summary>
        /// <param name="value"></param>
        public HMACHashedValue(string value) : this(value, 0) { }

        /// <summary>
        /// Construct wth value to hash and hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="algorithm"></param>
        public HMACHashedValue(string value, HashAlgorithmType algorithm) : this(value, 0, algorithm.GetAlgorithmOrDefault()) { }

        /// <summary>
        /// Construct wth value to hash and hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="algorithm"></param>
        public HMACHashedValue(string value, HashAlgorithmName algorithm) : this(value, 0, algorithm) { }

        /// <summary>
        /// Construct with key and value to hash
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="isKeyBase64"></param>
        public HMACHashedValue(string value, string key, bool isKeyBase64) : this(value, key, isKeyBase64, 0) { }

        /// <summary>
        /// Construct with key, value to hash, and hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="isKeyBase64"></param>
        /// <param name="algorithm"></param>
        public HMACHashedValue(string value, string key, bool isKeyBase64, HashAlgorithmType algorithm) : this(value, key, isKeyBase64, 0, algorithm.GetAlgorithmOrDefault()) { }

        /// <summary>
        /// Construct with key, value to hash, and hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="isKeyBase64"></param>
        /// <param name="algorithm"></param>
        public HMACHashedValue(string value, string key, bool isKeyBase64, HashAlgorithmName algorithm) : this(value, key, isKeyBase64, 0, algorithm) { }

        /// <summary>
        /// Construct with salt and value to hash
        /// </summary>
        /// <param name="value"></param>
        /// <param name="saltSize"></param>
        public HMACHashedValue(string value, int saltSize) : this(value, saltSize, HashAlgorithmName.SHA256) { }

        /// <summary>
        /// Construct with salt, value to hash, hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="saltSize"></param>
        /// <param name="algorithm"></param>
        public HMACHashedValue(string value, int saltSize, HashAlgorithmType algorithm) : this(value, saltSize, algorithm.GetAlgorithmOrDefault()) { }

        /// <summary>
        /// Construct with salt, value to hash, hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="saltSize"></param>
        /// <param name="algorithm"></param>
        public HMACHashedValue(string value, int saltSize, HashAlgorithmName algorithm)
        {
            Algorithm = algorithm;
            SaltByteSize = saltSize;
            SetHash(value);
        }

        /// <summary>
        /// Construct with salt, key, value to hash
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="isKeyBase64"></param>
        /// <param name="saltSize"></param>
        public HMACHashedValue(string value, string key, bool isKeyBase64, int saltSize) : this(value, key, isKeyBase64, saltSize, HashAlgorithmName.SHA256) { }

        /// <summary>
        /// Construct with salt, key, value to hash, hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="isKeyBase64"></param>
        /// <param name="saltSize"></param>
        /// <param name="algorithm"></param>
        public HMACHashedValue(string value, string key, bool isKeyBase64, int saltSize, HashAlgorithmType algorithm) : this(value, key, isKeyBase64, saltSize, algorithm.GetAlgorithmOrDefault()) { }

        /// <summary>
        /// Construct with salt, key, value to hash, hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="isKeyBase64"></param>
        /// <param name="saltSize"></param>
        /// <param name="algorithm"></param>
        public HMACHashedValue(string value, string key, bool isKeyBase64, int saltSize, HashAlgorithmName algorithm)
        {
            Algorithm = algorithm;
            SaltByteSize = saltSize;
            IsKeyBase64 = isKeyBase64;
            Key = key;
            SetHash(value);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Is key base 64
        /// </summary>
        public bool IsKeyBase64 { get; set; }

        /// <summary>
        /// Key for hash
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Byte array interface for key
        /// </summary>
        [XmlIgnore]
        public byte[] KeyBytes
        {
            get
            {
                if (Key == null) { return null; }
                return IsKeyBase64 ? Convert.FromBase64String(Key) : Encoding.UTF8.GetBytes(Key);
            }
            set
            {
                if (value != null)
                {
                    Key = IsKeyBase64 ? Convert.ToBase64String(value) : Encoding.UTF8.GetString(value);
                }
            }
        }


        /// <summary>
        /// Hash algorithm string name
        /// </summary>
        public new string AlgorithmName
        {
            get => "HMAC-" + base.AlgorithmName;
            set
            {
                if(value != null)
                {
                    value = value.Replace("HMAC-", "").Replace("HMAC", "");
                }
                base.AlgorithmName = value;
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Create hash for value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public new void SetHash(string value)
        {
            // Only set hash for non-null value
            if (value != null)
            {
                // Create hashing algorithm
                HMAC algorithm = HMAC.Create(AlgorithmName.Replace("-", ""));
                // Set saved key
                if (!string.IsNullOrEmpty(Key))
                {
                    algorithm.Key = KeyBytes;
                }
                algorithm.Initialize();
                // Get value bytes
                byte[] valueBytes = Encoding.UTF8.GetBytes(Salt + value);
                // Compute hash
                algorithm.ComputeHash(valueBytes);
                HashBytes = algorithm.Hash;
                // Set key if not already set
                if (string.IsNullOrEmpty(Key))
                {
                    IsKeyBase64 = true;
                    KeyBytes = algorithm.Key;
                }
            }
        }

        /// <summary>
        /// Check if provided object's hash equals this hash
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // Return false if hash is null
            if (string.IsNullOrWhiteSpace(Hash)) { return false; }
            // Set hashed value to compare
            HMACHashedValue toCompare = null;
            if (obj.GetType() == GetType())
            {
                toCompare = (HMACHashedValue)obj;
            }
            if (obj.GetType() == typeof(string))
            {
                toCompare = new HMACHashedValue()
                {
                    Algorithm = this.Algorithm,
                    Salt = this.Salt,
                    Key = this.Key,
                    IsKeyBase64 = this.IsKeyBase64
                };
                toCompare.SetHash((string)obj);
            }
            // Return comparison result
            if (toCompare != null && toCompare.Hash != null)
            {
                return SlowEquals(toCompare.HashBytes, HashBytes);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Hash code override
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Return properties
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{AlgorithmName} : {Hash}";
        }

        #endregion
    }
}
