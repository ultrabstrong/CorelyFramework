using Corely.Security.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Corely.DocuWareService
{
    [Serializable]
    public class ServiceConfig
    {
        /// <summary>
        /// Service Credentials
        /// </summary>
        public GeneralCredentials SvcCredentials
        {
            get => _svcCredentials;
            set
            {
                _svcCredentials = value;
                if (!string.IsNullOrWhiteSpace(CredentialsKey))
                {
                    _svcCredentials?.SetEncryptionKey(CredentialsKey);
                }
            }
        }
        private GeneralCredentials _svcCredentials = null;

        /// <summary>
        /// DocuWare Credential List
        /// </summary>
        public List<GeneralCredentials> DWCredentials
        {
            get => _dwCredentials;
            set
            {
                _dwCredentials = value;
                if (!string.IsNullOrWhiteSpace(CredentialsKey))
                {
                    _dwCredentials?.ForEach(m => m?.SetEncryptionKey(CredentialsKey));
                }
            }
        }
        private List<GeneralCredentials> _dwCredentials = new List<GeneralCredentials>();

        /// <summary>
        /// Credentials encryption key
        /// </summary>
        public string CredentialsKey
        {
            get => _credentialsKey;
            set
            {
                _credentialsKey = value;
                if (!string.IsNullOrWhiteSpace(_credentialsKey))
                {
                    SvcCredentials?.SetEncryptionKey(_credentialsKey);
                    DWCredentials?.ForEach(m => m?.SetEncryptionKey(_credentialsKey));
                }
            }
        }
        private string _credentialsKey = "";

        /// <summary>
        /// API Key
        /// </summary>
        public string APIKey { get; set; }

        /// <summary>
        /// Log level
        /// </summary>
        public string LogLevel { get; set; }

        /// <summary>
        /// Number of days token should be valid for
        /// </summary>
        public int TokenValidDays { get; set; }

    }
}