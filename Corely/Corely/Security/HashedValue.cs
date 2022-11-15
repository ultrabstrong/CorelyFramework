using Corely.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Corely.Security
{
    [Serializable]
    public class HashedValue
    {
        #region Constructor

        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public HashedValue() { }

        /// <summary>
        /// Construct with value to hash
        /// </summary>
        /// <param name="value"></param>
        public HashedValue(string value) : this(value, 0) { }

        /// <summary>
        /// Construct wth value to hash and hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="algorithm"></param>
        public HashedValue(string value, HashAlgorithmType algorithm) : this(value, 0, algorithm.GetAlgorithmOrDefault()) { }

        /// <summary>
        /// Construct wth value to hash and hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="algorithm"></param>
        public HashedValue(string value, HashAlgorithmName algorithm) : this(value, 0, algorithm) { }

        /// <summary>
        /// Construct with salt and value to hash
        /// </summary>
        /// <param name="value"></param>
        /// <param name="saltSize"></param>
        public HashedValue(string value, int saltSize) : this(value, saltSize, HashAlgorithmName.SHA256) { }

        /// <summary>
        /// Construct with salt, value to hash, and hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="saltSize"></param>
        /// <param name="algorithm"></param>
        public HashedValue(string value, int saltSize, HashAlgorithmType algorithm) : this(value, saltSize, algorithm.GetAlgorithmOrDefault()) { }

        /// <summary>
        /// Construct with salt, value to hash, and hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="saltSize"></param>
        /// <param name="algorithm"></param>
        public HashedValue(string value, int saltSize, HashAlgorithmName algorithm)
        {
            Algorithm = algorithm;
            SaltByteSize = saltSize;
            SetHash(value);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Hashed value
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Byte array interface for hash
        /// </summary>
        [XmlIgnore]
        public byte[] HashBytes 
        {
            get
            {
                if(Hash == null) { return null; }
                return Convert.FromBase64String(Hash);
            }
            set
            {
                if(value != null)
                {
                    Hash = Convert.ToBase64String(value);
                }
            }
        }

        /// <summary>
        /// Salt used to randomize hash
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Byte array interface for salt
        /// </summary>
        [XmlIgnore]
        public byte[] SaltBytes
        {
            get
            {
                if (Salt == null) { return null; }
                return Convert.FromBase64String(Salt);
            }
            set
            {
                if (value != null)
                {
                    Salt = Convert.ToBase64String(value);
                }
            }
        }

        /// <summary>
        /// Byte size for generating salt
        /// </summary>
        [XmlIgnore]
        public int SaltByteSize
        {
            get => SaltBytes.Length;
            set
            {
                if (value > -1)
                {
                    // Generate a salt for randomizing hash
                    byte[] saltBytes = new byte[value];
                    RNGCryptoServiceProvider cryptoprovider = new RNGCryptoServiceProvider();
                    cryptoprovider.GetBytes(saltBytes);
                    SaltBytes = saltBytes;
                }
            }
        }

        /// <summary>
        /// Hash algorithm to use
        /// </summary>
        [XmlIgnore]
        public HashAlgorithmName Algorithm { get; set; } = HashAlgorithmName.SHA256;

        /// <summary>
        /// Hash algorithm string name
        /// </summary>
        public string AlgorithmName
        {
            get => Algorithm.Name;
            set
            {
                if(value == HashAlgorithmName.MD5.Name ||
                   value == HashAlgorithmName.SHA1.Name ||
                   value == HashAlgorithmName.SHA256.Name ||
                   value == HashAlgorithmName.SHA384.Name ||
                   value == HashAlgorithmName.SHA512.Name)
                {
                    Algorithm = new HashAlgorithmName(value);
                }             
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create hash for value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetHash(string value)
        {
            // Only set hash for non-null value
            if(value != null)
            {
                // Create hashing algorithm
                HashAlgorithm algorithm = HashAlgorithm.Create(AlgorithmName);
                algorithm.Initialize();
                // Get value bytes
                byte[] valueBytes = Encoding.UTF8.GetBytes(Salt + value);
                // Compute hash
                algorithm.ComputeHash(valueBytes);
                HashBytes = algorithm.Hash;
            }
        }

        /// <summary>
        /// Verify hashed values are equivilant
        /// </summary>
        /// <param name="hash1"></param>
        /// <param name="hash2"></param>
        /// <returns></returns>
        internal bool SlowEquals(byte[] hash1, byte[] hash2)
        {
            var diff = (uint)hash1.Length ^ (uint)hash2.Length;
            for (int i = 0; i < hash1.Length && i < hash2.Length; i++)
            {
                diff |= (uint)(hash1[i] ^ hash2[i]);
            }
            return diff == 0;
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
            HashedValue toCompare = null;
            if (obj.GetType() == GetType())
            {
                toCompare = (HashedValue)obj;
            }
            if (obj.GetType() == typeof(string))
            {
                toCompare = new HashedValue()
                {
                    Algorithm = this.Algorithm,
                    Salt = this.Salt
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
