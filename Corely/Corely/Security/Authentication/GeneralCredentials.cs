using Corely.Core;
using System;

namespace Corely.Security.Authentication
{
    [Serializable]
    public class GeneralCredentials : IEncryptedValueProvider
    {
        #region Constructor

        /// <summary>
        /// Constructor for serialization and auto-generated key
        /// </summary>
        public GeneralCredentials() { }

        /// <summary>
        /// Construct with key
        /// </summary>
        /// <param name="key"></param>
        public GeneralCredentials(string key) : this(key, "") { }

        /// <summary>
        /// Construct with key and password
        /// </summary>
        /// <param name="key"></param>
        public GeneralCredentials(string key, string password)
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
        /// Credentials name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Credentials Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Windows Domain
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Tenant
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public AESValue Password { get; set; } = new AESValue();

        /// <summary>
        /// Additional authentication secrets
        /// </summary>
        public AESValues AuthSecrets { get; set; } = new AESValues();

        /// <summary>
        /// Additional authentication values
        /// </summary>
        public NamedValues AuthValues { get; set; } = new NamedValues();

        /// <summary>
        /// Token
        /// </summary>
        public AuthenticationToken Token { get; set; } = new AuthenticationToken();

        #endregion

        #region Methods

        /// <summary>
        /// Return new credentials without encrypted data
        /// </summary>
        /// <returns></returns>
        public GeneralCredentials GetWithoutAESValues()
        {
            return new GeneralCredentials()
            {
                Name = this.Name,
                Id = this.Id,
                Host = this.Host,
                Port = this.Port,
                Domain = this.Domain,
                Tenant = this.Tenant,
                Username = this.Username
            };
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
            Token?.SetEncryptionKey(key);
            AuthSecrets?.SetEncryptionKey(key);
        }

        #endregion

    }
}
