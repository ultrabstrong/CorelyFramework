using Corely.Data.Dates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Security.Authentication
{
    [Serializable]
    public class AuthenticationToken : IEncryptedValueProvider
    {
        #region Constructors

        /// <summary>
        /// Constructor for serialization
        /// </summary>
        public AuthenticationToken() { }

        /// <summary>
        /// Create with key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expires"></param>
        public AuthenticationToken(string key) 
        {
            SetEncryptionKey(key);
        }

        /// <summary>
        /// Create token with expiration date
        /// </summary>
        /// <param name="token"></param>
        /// <param name="expires"></param>
        public AuthenticationToken(string token, DateTime expires) : this(token, expires, AESEncryption.CreateRandomBase64Key()) { }

        /// <summary>
        /// Create token with timespan-calculated expiration date
        /// </summary>
        /// <param name="token"></param>
        /// <param name="expires"></param>
        public AuthenticationToken(string token, TimeSpan expiresin) : this(token, expiresin, AESEncryption.CreateRandomBase64Key()) { }

        /// <summary>
        /// Create token with expiration date
        /// </summary>
        /// <param name="token"></param>
        /// <param name="expires"></param>
        public AuthenticationToken(string token, DateTime expires, string key) : this(key)
        {
            Token.DecryptedValue = token;
            Expires = expires;
        }

        /// <summary>
        /// Create token with timespan-calculated expiration date
        /// </summary>
        /// <param name="token"></param>
        /// <param name="expires"></param>
        public AuthenticationToken(string token, TimeSpan expiresin, string key)  : this(key)
        {
            Token.DecryptedValue = token;
            Expires = DateTime.Now.Add(expiresin);
        }

        /// <summary>
        /// Create nonexpiring token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="expires"></param>
        public AuthenticationToken(string token, string key) : this(key)
        {
            Token.DecryptedValue = token;
            IsNonExpiring = true;
        }

        #endregion

        #region Properties 

        /// <summary>
        /// Token string
        /// </summary>
        public AESValue Token { get; set; } = new AESValue();

        /// <summary>
        /// Expiration datetime
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// ? Does token expire
        /// </summary>
        public bool IsNonExpiring { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get milliseconds till expiration
        /// </summary>
        /// <returns></returns>
        public double MillisecondsTillExpiration() => (Expires - DateTime.Now).TotalMilliseconds;

        /// <summary>
        /// Check if token will remain valid for the given time interval
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool IsValidForInterval(TimeSpan interval) => MillisecondsTillExpiration() > interval.TotalMilliseconds;

        /// <summary>
        /// Check if token is valid and not expired
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (Token == null || string.IsNullOrWhiteSpace(Token.EncryptedValue) || Expires < DateTime.UtcNow)
            {
                return false;
            }
            return true;
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
            Token?.SetEncryptionKey(key);
        }

        #endregion

    }
}
