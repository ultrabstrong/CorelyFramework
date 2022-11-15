using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Corely.Security
{
    [Serializable]
    public class AESValues : List<AESValue>, IEncryptedValueProvider
    {
        #region Constructor

        /// <summary>
        /// Empty constructor for serialization
        /// </summary>
        public AESValues() { }

        /// <summary>
        /// Parameterized constructor for easy initialization
        /// </summary>
        /// <param name="_key"></param>
        public AESValues(string key)
        {
            SetEncryptionKey(key);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Key for encrypting and decrypting
        /// </summary>
        [XmlIgnore]
        public string Key
        {
            get => _key;
            private set
            {
                // Set this key
                _key = value;
                // Set key for all values
                foreach (AESValue aesvalue in this) 
                { 
                    aesvalue?.SetEncryptionKey(_key); 
                }
            }
        }
        private string _key;

        /// <summary>
        /// String accessor
        /// </summary>
        /// <param name="valuename"></param>
        /// <returns></returns>
        public string this[string valuename]
        {
            get => this.FirstOrDefault(m => m.Name == valuename)?.DecryptedValue;
            set
            {
                // Get value by value name if it exists
                AESValue existingValue = this.FirstOrDefault(m => m.Name == valuename);
                if (existingValue != null)
                {
                    // Update existing value
                    existingValue.DecryptedValue = value;
                }
                else
                {
                    // Add new value
                    Add(new AESValue(value, Key, valuename));
                }
            }
        }

        /// <summary>
        /// Index accessor
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public new AESValue this[int index]
        {
            get => base[index];
            set
            {
                if (!string.IsNullOrWhiteSpace(Key))
                {
                    value?.SetEncryptionKey(Key);
                }
                base[index] = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add AES value
        /// </summary>
        /// <param name="value"></param>
        public new void Add(AESValue value)
        {
            if (!string.IsNullOrWhiteSpace(Key))
            {
                value?.SetEncryptionKey(Key);
            }
            base.Add(value);
        }

        /// <summary>
        /// Create and add AES value
        /// </summary>
        /// <param name="valuename"></param>
        /// <param name="decyrptedvalue"></param>
        public void Add(string valuename, string decyrptedvalue)
        {
            Add(new AESValue(decyrptedvalue, Key, valuename));
        }

        /// <summary>
        /// Remove value for value name
        /// </summary>
        /// <param name="valuename"></param>
        public void Remove(string valuename)
        {
            this.RemoveAll(m => m.Name == valuename);
        }

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
