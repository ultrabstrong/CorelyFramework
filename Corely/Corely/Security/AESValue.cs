using System;
using System.Xml.Serialization;
using rm = Corely.Resources.Security.AESValue;

namespace Corely.Security
{
    [Serializable]
    public class AESValue : IEncryptedValueProvider
    {
        #region Constructor

        /// <summary>
        /// Empty constructor for serialization
        /// </summary>
        public AESValue() { }

        /// <summary>
        /// Parameterized constructor for easy initialization
        /// </summary>
        /// <param name="decryptedvalue"></param>
        /// <param name="key"></param>
        public AESValue(string decryptedvalue, string key)
        {
            SetEncryptionKey(key);
            DecryptedValue = decryptedvalue;
        }

        /// <summary>
        /// Parameterized constructor with name for easy initialization
        /// </summary>
        /// <param name="decryptedvalue"></param>
        /// <param name="key"></param>
        /// <param name="name"></param>
        public AESValue(string decryptedvalue, string key, string name) : this(decryptedvalue, key)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of this encrypted value
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Encrypted value
        /// </summary>
        public string EncryptedValue { get; set; }

        /// <summary>
        /// Decrypted value
        /// </summary>
        [XmlIgnore]
        public string DecryptedValue
        {
            get
            {
                // Validate key
                if (string.IsNullOrWhiteSpace(Key))
                {
                    throw new Exception($"{nameof(AESValue)} - {rm.setKeyFirst}");
                }
                if (string.IsNullOrWhiteSpace(EncryptedValue))
                {
                    return "";
                }
                // Get decrypted value
                return AESEncryption.Decrypt(Key, EncryptedValue);
            }
            set
            {
                // Validate key
                if (string.IsNullOrWhiteSpace(Key))
                {
                    throw new Exception($"{nameof(AESValue)} - {rm.setKeyFirst}");
                }
                // Set encrypted value
                EncryptedValue = AESEncryption.Encrypt(Key, value ?? "");
            }
        }

        /// <summary>
        /// Key for encrypting and decrypting
        /// </summary>
        [XmlIgnore]
        public string Key
        {
            get { return _key; }
            private set
            {
                // Validate key
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception($"{nameof(AESValue)} - {rm.keyNotWhitespace}");
                }
                // Don't update if new key is same as old key
                if(_key == value) { return; }
                // Re-encrypt for new key if key is already set
                if (!string.IsNullOrWhiteSpace(_key) && !string.IsNullOrWhiteSpace(EncryptedValue))
                {
                    EncryptedValue = AESEncryption.Encrypt(value, DecryptedValue);
                }
                // Set key
                _key = value;

            }
        }
        private string _key;

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
        /// Method for setting encryption key
        /// </summary>
        /// <param name="key"></param>
        public void SetEncryptionKey(string key)
        {
            Key = key;
        }

        #endregion

    }
}
