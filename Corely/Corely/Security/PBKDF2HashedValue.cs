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
    public class PBKDF2HashedValue : HashedValue
    {
        private const int defaultSalt = 32, defaultHash = 32, defaultIterations = 100000;

        #region Constructor

        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public PBKDF2HashedValue() { }

        /// <summary>
        /// Construct with value to hash
        /// </summary>
        /// <param name="value"></param>
        /// <param name="saltSize"></param>
        /// <param name="hashSize"></param>
        /// <param name="iterations"></param>
        public PBKDF2HashedValue(string value, int saltSize = defaultSalt, int hashSize = defaultHash, int iterations = defaultIterations) : this(value, HashAlgorithmName.SHA256, saltSize, hashSize, iterations) { }

        /// <summary>
        /// Construct wth value to hash and hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="algorithm"></param>
        /// <param name="saltSize"></param>
        /// <param name="hashSize"></param>
        /// <param name="iterations"></param>
        public PBKDF2HashedValue(string value, HashAlgorithmType algorithm, int saltSize = defaultSalt, int hashSize = defaultHash, int iterations = defaultIterations) : this(value, algorithm.GetAlgorithmOrDefault(), saltSize, hashSize, iterations) { }
        
        /// <summary>
        /// Construct wth value to hash and hash algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="algorithm"></param>
        /// <param name="saltSize"></param>
        /// <param name="hashSize"></param>
        /// <param name="iterations"></param>
        public PBKDF2HashedValue(string value, HashAlgorithmName algorithm, int saltSize = defaultSalt, int hashSize = defaultHash, int iterations = defaultIterations)
        {
            Algorithm = algorithm;
            SaltByteSize = saltSize;
            HashByteSize = hashSize;
            Iterations = iterations;
            SetHash(value);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of hash iterations
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// Byte size for generating hash
        /// </summary>
        [XmlIgnore]
        public int HashByteSize
        {
            get => HashBytes.Length;
            set
            {
                if (value > -1)
                {
                    HashBytes = new byte[value];
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
        public new void SetHash(string value)
        {
            // Only set hash for non-null value
            if (value != null)
            {
                // Hash the value
                Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(value, SaltBytes, Iterations, Algorithm);
                HashBytes = pbkdf2.GetBytes(HashByteSize);
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
            PBKDF2HashedValue toCompare = null;
            if(obj.GetType() == GetType())
            {
                toCompare = (PBKDF2HashedValue)obj;
            }
            if(obj.GetType() == typeof(string))
            {
                toCompare = new PBKDF2HashedValue()
                {
                    Salt = this.Salt,
                    Iterations = this.Iterations,
                    HashByteSize = this.HashByteSize,
                    Algorithm = this.Algorithm
                };
                toCompare.SetHash((string)obj);
            }
            // Return comparison result
            if(toCompare != null && toCompare.Hash != null)
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
            return $"PBKDF2-HMAC-{AlgorithmName} : {Hash}";
        }

        #endregion
    }
}
