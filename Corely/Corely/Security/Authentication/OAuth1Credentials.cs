using Corely.Connections;
using Corely.Core;
using Corely.Data.Encoding;
using Corely.Data.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Corely.Security.Authentication
{
    [Serializable]
    public class OAuth1Credentials : IEncryptedValueProvider
    {
        // OAuth constant version
        public const string OAuthVersionId = "1.0";

        // OAuth constant names
        private const string 
            OAuthCallbackKey    = "oauth_callback",
            OAuthConsumerKey    = "oauth_consumer_key",
            OAuthNonce          = "oauth_nonce",
            OAuthRealm          = "realm",
            OAuthSignature      = "oauth_signature",
            OAuthSignatureMethod = "oauth_signature_method",
            OAuthTimestamp      = "oauth_timestamp",
            OAuthToken          = "oauth_token",
            OAuthVersionName    = "oauth_version";

        #region Constructors


        /// <summary>
        /// Constructor for serialization and auto-generated key
        /// </summary>
        public OAuth1Credentials() { }

        /// <summary>
        /// Construct with key
        /// </summary>
        /// <param name="key"></param>
        public OAuth1Credentials(string key) : this(key, "") { }

        /// <summary>
        /// Construct with key and consumer secret
        /// </summary>
        /// <param name="key"></param>
        public OAuth1Credentials(string key, string consumerSecret) : this(key, consumerSecret, "") { }

        /// <summary>
        /// Construct with key and password
        /// </summary>
        /// <param name="key"></param>
        public OAuth1Credentials(string key, string consumerSecret, string tokenSecret)
        {
            SetEncryptionKey(key);
            ConsumerSecret.DecryptedValue = consumerSecret;
            TokenSecret.DecryptedValue = tokenSecret;
        }

        #endregion

        #region Properties

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
        /// Consumer Key
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Consumer secret
        /// </summary>
        public AESValue ConsumerSecret { get; set; } = new AESValue();

        /// <summary>
        /// Token secret
        /// </summary>
        public AESValue TokenSecret { get; set; } = new AESValue();

        /// <summary>
        /// Realm
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// Nonce length
        /// </summary>
        public int NonceLength { get; set; } = 11;

        /// <summary>
        /// Callback URL
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// Signature method
        /// </summary>
        public HashAlgorithmType SignatureMethod { get; set; } = HashAlgorithmType.Sha256;

        #endregion

        #region Structs

        /// <summary>
        /// Struct for returning signature base string
        /// </summary>
        public struct OAuth1SignatureBaseString
        {
            internal OAuth1SignatureBaseString(string nonce, string unixTimestamp, string signatureBaseString)
            {
                Nonce = nonce;
                UnixTimestamp = unixTimestamp;
                SignatureBaseString = signatureBaseString;
            }
            public string Nonce { get; private set; }
            public string UnixTimestamp { get; private set; }
            public string SignatureBaseString { get; private set; }
        }

        /// <summary>
        /// Struct for returning signature
        /// </summary>
        public struct OAuth1Signature
        {
            internal OAuth1Signature(string signature) : this(signature, null)
            {
                Signature = signature;
            }
            internal OAuth1Signature(string signature, OAuth1SignatureBaseString? signatureBaseString)
            {
                Signature = signature;
                SignatureBaseString = signatureBaseString;
            }
            public string Signature { get; private set; }
            public OAuth1SignatureBaseString? SignatureBaseString { get; private set; }
        }

        /// <summary>
        /// Struct for returning authorization
        /// </summary>
        public struct OAuth1Authorization
        {
            internal OAuth1Authorization(string authorization, OAuth1Signature signature)
            {
                Authorization = authorization;
                Signature = signature;
            }
            public string Authorization { get; private set; }
            public OAuth1Signature Signature { get; private set; }
        }

        #endregion

        #region Base Methods

        /// <summary>
        /// Get authorization header
        /// </summary>
        /// <param name="requestPath"></param>
        /// <param name="parameters"></param>
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public OAuth1Authorization GetAuthorizationHeader(HttpMethod method, string requestPath, NamedValues parameters)
        {
            OAuth1Signature signature = GetSignature(method, requestPath, parameters);
            NamedValues authValues = new NamedValues();
            authValues.Add(OAuthConsumerKey, ConsumerKey);
            authValues.Add(OAuthNonce, signature.SignatureBaseString?.Nonce ?? GetNonce());
            authValues.Add(OAuthTimestamp, signature.SignatureBaseString?.UnixTimestamp ?? GetUnixTimestampString());
            authValues.Add(OAuthVersionName, OAuthVersionId);
            authValues.Add(OAuthSignatureMethod, $"HMAC-{SignatureMethod}".ToUpper());
            authValues.Add(OAuthSignature, signature.Signature.UrlEncode());
            // Add optional oauth parameters
            if (!string.IsNullOrWhiteSpace(Token)) { authValues.Add(OAuthToken, Token); }
            if (!string.IsNullOrWhiteSpace(Realm)) { authValues.Add(OAuthRealm, Realm); }
            // Sort auth values according to name
            authValues.Sort();
            // Create and return auth string
            return new OAuth1Authorization("OAuth " + string.Join(",", authValues.Select(m => $"{m.Name}=\"{m.Value}\"")), signature);
        }

        /// <summary>
        /// Get signature for named value parameters
        /// </summary>
        /// <param name="requestPath"></param>
        /// <param name="parameters"></param>
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public OAuth1Signature GetSignature(HttpMethod method, string requestPath, NamedValues parameters)
        {
            // PLAIN_TEXT - Return signature key as signature if no authentication is selected
            if (SignatureMethod == HashAlgorithmType.None)
            {
                return new OAuth1Signature($"{ConsumerSecret?.DecryptedValue}&{TokenSecret?.DecryptedValue}".UrlEncode());
            }
            // Get signature base string to digest
            OAuth1SignatureBaseString signatureBaseString = GetSignatureBaseString(method, requestPath, parameters);
            // Create signature key
            string signatureKey = $"{ConsumerSecret?.DecryptedValue?.UrlEncode()}&{TokenSecret?.DecryptedValue?.UrlEncode()}";
            // Create and return signature
            HMACHashedValue signature = new HMACHashedValue(signatureBaseString.SignatureBaseString, signatureKey, false, SignatureMethod);
            return new OAuth1Signature(signature.Hash, signatureBaseString);
        }

        /// <summary>
        /// Get signature for named value parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="parameters"></param>
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public OAuth1SignatureBaseString GetSignatureBaseString(HttpMethod method, string requestPath, NamedValues parameters)
        {
            // Add required oauth parameters
            if (parameters == null) { parameters = new NamedValues(); }
            parameters.Add(OAuthConsumerKey, ConsumerKey);
            parameters.Add(OAuthNonce, GetNonce());
            parameters.Add(OAuthTimestamp, GetUnixTimestampString());
            parameters.Add(OAuthVersionName, OAuthVersionId);
            parameters.Add(OAuthSignatureMethod, $"HMAC-{SignatureMethod}".ToUpper());
            // Add optional oauth parameters
            if (!string.IsNullOrWhiteSpace(Token)) { parameters.Add(OAuthToken, Token); }
            // Sort parameters according to name
            parameters.Sort();
            // Create signature base string parts
            string methodString = method.ToString().ToUpper();
            string url = $"{Host.TrimEnd('/')}/{requestPath?.TrimStart('/')}".UrlEncode();
            string paramString = string.Join("&", parameters.Select(m => $"{m.Name}={m.Value}")).UrlEncode();
            // Return full signature base string
            return new OAuth1SignatureBaseString(parameters[OAuthNonce], parameters[OAuthTimestamp], $"{methodString}&{url}&{paramString}");
        }

        /// <summary>
        /// Get unix timestamp as string
        /// </summary>
        /// <returns></returns>
        public string GetUnixTimestampString() => GetUnixTimestamp().ToString();

        /// <summary>
        /// Get unix timestamp
        /// </summary>
        /// <returns></returns>
        public long GetUnixTimestamp() => DateTimeOffset.Now.ToUnixTimeSeconds();

        /// <summary>
        /// Create string nonce
        /// </summary>
        /// <returns></returns>
        public string GetNonce() => RandomStringGenerator.GetString(NonceLength);

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
            ConsumerSecret?.SetEncryptionKey(key);
            TokenSecret?.SetEncryptionKey(key);
        }

        #endregion

        #region Easy Access Interface Methods

        /// <summary>
        /// Get authorization header
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="parameters"></param>
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public OAuth1Authorization GetAuthorizationHeader(HttpMethod method, string requestPath)
        {
            return GetAuthorizationHeader(method, requestPath, new NamedValues());
        }

        /// <summary>
        /// Get authorization header for dictionary of parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="parameters"></param>
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public OAuth1Authorization GetAuthorizationHeader(HttpMethod method, string requestPath, Dictionary<string, string> parameters)
        {
            return GetAuthorizationHeader(method, requestPath, new NamedValues(parameters));
        }

        /// <summary>
        /// Get authorization header for http parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="parameters"></param>
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public OAuth1Authorization GetAuthorizationHeader(HttpMethod method, string requestPath, HttpParameters parameters)
        {
            return GetAuthorizationHeader(method, requestPath, parameters?.ToNamedValues());
        }

        /// <summary>
        /// Get signature
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="parameters"></param>
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public OAuth1Signature GetSignature(HttpMethod method, string requestPath)
        {
            return GetSignature(method, requestPath, new NamedValues());
        }

        /// <summary>
        /// Get signature for dictionary of parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="parameters"></param>
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public OAuth1Signature GetSignature(HttpMethod method, string requestPath, Dictionary<string, string> parameters)
        {
            return GetSignature(method, requestPath, new NamedValues(parameters));
        }

        /// <summary>
        /// Get signature for http parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="parameters"></param>
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public OAuth1Signature GetSignature(HttpMethod method, string requestPath, HttpParameters parameters)
        {
            return GetSignature(method, requestPath, parameters?.ToNamedValues());
        }

        /// <summary>
        /// Get signature base string
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="parameters"></param>
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public OAuth1SignatureBaseString GetSignatureBaseString(HttpMethod method, string requestPath)
        {
            return GetSignatureBaseString(method, requestPath, new NamedValues());
        }

        /// <summary>
        /// Get signature base string for dictionary of parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="parameters"></param>
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public OAuth1SignatureBaseString GetSignatureBaseString(HttpMethod method, string requestPath, Dictionary<string, string> parameters)
        {
            return GetSignatureBaseString(method, requestPath, new NamedValues(parameters));
        }

        /// <summary>
        /// Get signature base string for http parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="parameters"></param>
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public OAuth1SignatureBaseString GetSignatureBaseString(HttpMethod method, string requestPath, HttpParameters parameters)
        {
            return GetSignatureBaseString(method, requestPath, parameters?.ToNamedValues());
        }

        #endregion

    }
}
