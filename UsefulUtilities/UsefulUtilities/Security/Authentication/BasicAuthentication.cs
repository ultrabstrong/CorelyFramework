using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Security.Authentication
{
    [Serializable]
    public class BasicAuthentication : IEncryptedValueProvider
    {
        #region Constructor

        /// <summary>
        /// Constructor for serialization and auto-key generation
        /// </summary>
        public BasicAuthentication() { }

        /// <summary>
        /// Construct with key
        /// </summary>
        /// <param name="key"></param>
        public BasicAuthentication(string key) : this("", "", key) { }

        /// <summary>
        /// Construct with username, password, and auto-generated key
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public BasicAuthentication(string username, string password) : this(username, password, AESEncryption.CreateRandomBase64Key()) { }

        /// <summary>
        /// Construct with username, password, and key
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="key"></param>
        public BasicAuthentication(string username, string password, string key)
        {
            SetEncryptionKey(key);
            Password.DecryptedValue = password;
            Username = username;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public AESValue Password { get; set; } = new AESValue();

        #endregion

        #region Methods

        /// <summary>
        /// Get basic authentication string
        /// </summary>
        /// <returns></returns>
        public string GetBasicAuthString()
        {
            string baseauth = UsefulUtilities.Data.Encoding.Base64String.Base64Encode($"{Username}:{Password.DecryptedValue}");
            return $"Basic {baseauth}";
        }

        /// <summary>
        /// Create new encryption key
        /// </summary>
        public void CreateEncryptionKey()
        {
            SetEncryptionKey(AESEncryption.CreateRandomBase64Key());
        }

        /// <summary>
        /// Set encryption key
        /// </summary>
        /// <param name="key"></param>
        public void SetEncryptionKey(string key)
        {
            Password?.SetEncryptionKey(key);
        }

        #endregion
    }
}
