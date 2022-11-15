
using System;

namespace UsefulUtilities.Security.Authentication
{
    [Serializable]
    public class OdbcCredentials : IEncryptedValueProvider
    {
        #region Constructor

        /// <summary>
        /// Constructor for serialization and auto-generated key
        /// </summary>
        public OdbcCredentials() { }

        /// <summary>
        /// Construct with key
        /// </summary>
        /// <param name="_key"></param>
        public OdbcCredentials(string key) : this("", key) { }

        /// <summary>
        /// Construct with key
        /// </summary>
        /// <param name="_key"></param>
        public OdbcCredentials(string password, string key)
        {
            SetEncryptionKey(key);
            Password.DecryptedValue = password;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Authenticaiton type
        /// </summary>
        public AuthenticationType AuthenticationType { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public AESValue Password { get; set; } = new AESValue();

        /// <summary>
        /// DSN name
        /// </summary>
        public string DSN { get; set; }

        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Database
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; } = -1;

        #endregion

        #region Methods

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
