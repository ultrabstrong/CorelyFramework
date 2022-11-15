using Corely.Connections.Proxies;
using Corely.Connections;
using Corely.Kingstone.Connection;
using Corely.Security;
using Corely.Security.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Corely.Kingstone.Core;

namespace Corely.Kingstone
{
    [Serializable]
    public class KingstoneConnection
    {
        #region Constructor / Initialization

        /// <summary>
        /// Construct with secret object initialized
        /// </summary>
        public KingstoneConnection()
        {
            AuthenticationSecrets = new AESValues();
            AuthSecretsKey = AESEncryption.CreateRandomBase64Key();
        }

        /// <summary>
        /// Contstruct with connection information set
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="apikey"></param>
        /// <param name="apisecret"></param>
        public KingstoneConnection(string baseAddress, string username, string password, string apikey) : this()
        {
            BaseAddress = baseAddress;
            Username = username;
            AuthenticationSecrets[Password] = password;
            AuthenticationSecrets[ApiKey] = apikey;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Base address of web service
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// Username for connecting to web service
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Encrypted authentication values
        /// </summary>
        public AESValues AuthenticationSecrets { get; set; }

        /// <summary>
        /// Key for authentication secrets
        /// </summary>
        public string AuthSecretsKey
        {
            get { return _authSecretsKey; }
            set
            {
                _authSecretsKey = value;
                AuthenticationSecrets?.SetEncryptionKey(_authSecretsKey);
            }
        }
        private string _authSecretsKey;

        /// <summary>
        /// Authenticated user returned from valid authentication
        /// </summary>
        [XmlIgnore]
        public AuthenticatedUser AuthenticatedUser { get; internal set; }

        /// <summary>
        /// Auth token returned from valid authentication
        /// </summary>
        [XmlIgnore]
        public AuthenticationToken AuthenticationToken { get; internal set; }

        /// <summary>
        /// Http proxy with base address for kingstone
        /// </summary>
        [XmlIgnore]
        public HttpProxy Proxy { get; internal set; }

        /// <summary>
        /// Strings for accessing authentication secrets
        /// </summary>
        private readonly string Password = "Password";
        private readonly string ApiKey = "ApiKey";

        /// <summary>
        /// API Endpoint links
        /// </summary>
        private static class Links
        {
            internal static readonly string Authenticate = "/api/v1/Authenticate";
            internal static readonly string GetToken = "/api/v1/GetToken";
            internal static readonly string Payment = "/api/v1/Policy/Payments";
            internal static readonly string Policy = "/api/v1/Policy";
        }

        #endregion

        #region Connection Methods

        /// <summary>
        /// Send authentication request
        /// </summary>
        public void Connect()
        {
            if(!IsConnected())
            {
                // Create proxy for web serivce
                Proxy = new HttpProxy();
                Proxy.Connect(BaseAddress);
                // Create authentication headers
                BasicAuthentication authentication = new BasicAuthentication(Username, AuthenticationSecrets?[Password]);
                Dictionary<string, string> authenticationHeaders = new Dictionary<string, string>()
                {
                    { "Authorization", authentication.GetBasicAuthString() },
                    { "Application", AuthenticationSecrets?[ApiKey] }
                };
                // Post to endpoint for authentication
                HttpResponseMessage result = Proxy.SendRequestForHttpResponse(Links.Authenticate, Corely.Connections.HttpMethod.Post, null, authenticationHeaders, null);
                // Read authentication json response
                string json = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                AuthenticatedUser = JsonConvert.DeserializeObject<AuthenticatedUser>(json);
                // Read header token and expiration into authentication token
                List<string> token = result.Headers.First(header => header.Key == "Token").Value.ToList();
                List<string> tokenexpiry = result.Headers.First(header => header.Key == "TokenExpiry").Value.ToList();
                AuthenticationToken = new AuthenticationToken(token[0], TimeSpan.Parse(tokenexpiry[0]));
            }
        }

        /// <summary>
        /// Refresh token if it's expiring within the given timespan
        /// </summary>
        /// <param name="timetoexpire"></param>
        public void RefreshTokenIfExpiring(TimeSpan timetoexpire)
        {
            if(AuthenticationToken != null && AuthenticationToken.IsValidForInterval(timetoexpire) == false)
            {
                RefreshToken();
            }
        }

        /// <summary>
        /// Send request to get new token
        /// </summary>
        public void RefreshToken()
        {
            // Refresh token by creating connection if there isn't a current connection
            if (!IsConnected()) { Connect(); }
            else
            {
                // Create headers
                Dictionary<string, string> authenticationHeaders = new Dictionary<string, string>()
                {
                    { "Token", AuthenticationToken.Token.DecryptedValue }
                };
                // Create parameters
                HttpParameters param = new HttpParameters(new Dictionary<string, string>()
                {
                    { "userId", Username },
                    { "password", AuthenticationSecrets?[Password] },
                });
                // Post to endpoint for authentication
                string result = Proxy.SendRequestForStringResult(Links.GetToken, Corely.Connections.HttpMethod.Post, null, authenticationHeaders, param);
                // Update auth token with non-expiring token result
                AuthenticationToken = new AuthenticationToken(result, DateTime.Now.AddYears(5));
            }
        }

        /// <summary>
        /// Clear the current authentication data
        /// </summary>
        /// <returns></returns>
        public void Disconnect()
        {
            Proxy = null;
            AuthenticationToken = null;
        }

        /// <summary>
        /// Check if auth token is valid
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            // Return false if token is invalid
            if (AuthenticationToken == null || AuthenticationToken.IsValid() == false)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if password is the same as the current password
        /// </summary>
        /// <param name="pw"></param>
        /// <returns></returns>
        public bool ValidatePassword(string pw) => pw == AuthenticationSecrets[Password];

        /// <summary>
        /// Set password
        /// </summary>
        /// <param name="pw"></param>
        /// <returns></returns>
        public void SetPassword(string pw)
        {
            AuthenticationSecrets[Password] = pw;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get all policies
        /// </summary>
        /// <returns></returns>
        public List<Policy> GetAllPolicies()
        {
            return GetAllPagedItems<Policy>(Links.Policy);
        }

        /// <summary>
        /// Post a payment
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        public string PostPayment(Payment payment)
        {
            return PostJsonForStringContent(Links.Payment, payment);
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Get all paged items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="link"></param>
        /// <returns></returns>
        internal List<T> GetAllPagedItems<T>(string link, HttpParameters parameter = null) where T : class
        {
            PagedResult<T> result = GetAllPagedResult<T>(link, parameter);
            return result.Items;
        }

        /// <summary>
        /// Get all paged results for link
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="link"></param>
        /// <param name="pagedResult"></param>
        /// <returns></returns>
        internal PagedResult<T> GetAllPagedResult<T>(string link, HttpParameters parameter = null) where T : class
        {
            PagedResult<T> result = GetPagedResult<T>(link, true, parameter);
            while (result.HasMore)
            {
                GetPagedResult<T>(link, true, parameter, result);
            }
            return result;
        }

        /// <summary>
        /// Get paged result for link
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="link"></param>
        /// <param name="pagedResult"></param>
        /// <returns></returns>
        internal PagedResult<T> GetPagedResult<T>(string link, bool appendChunck, HttpParameters parameter = null, PagedResult<T> pagedResult = null) where T : class
        {
            // Create result if this is a new request
            if (pagedResult == null) { pagedResult = new PagedResult<T>(0, 100); }
            // Set temporary paging parameters
            if(parameter == null) { parameter = new HttpParameters(null); }
            parameter.TempParameters = new Dictionary<string, string>()
            {
                { "offset", pagedResult.Skip.ToString() },
                { "limit", pagedResult.Take.ToString() }
            };
            // Deserialize response
            List<T> result = GetResult<List<T>>(link, parameter);
            // Add or set result to paged result
            if (appendChunck) { pagedResult.AddItems(result); }
            else { pagedResult.SetItems(result); }
            // Add or set deserialized result
            return pagedResult;
        }

        /// <summary>
        /// Get result from parameterized search
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="link"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        internal T GetResult<T>(string link, HttpParameters parameter = null)
        {
            try
            {
                // Make sure this is connected
                Connect();
                // Create cookie header
                Dictionary<string, string> header = new Dictionary<string, string>
                {
                    { "Token", AuthenticationToken.Token.DecryptedValue }
                };
                // Get result
                string resultString = Proxy.SendRequestForStringResult(link, Corely.Connections.HttpMethod.Get, null, header, parameter);
                // Deserialize response
                T result = JsonConvert.DeserializeObject<T>(resultString);
                return result;
            }
            catch (Exception ex)
            {
                if(ex.InnerException?.Message != null)
                {
                    // Thow exception of result can be parsed as an error object
                    List<ErrorResponse> errorResponse = JsonConvert.DeserializeObject<List<ErrorResponse>>(ex.InnerException.Message);
                    if (errorResponse != null) 
                    { 
                        throw new ErrorResponseException(errorResponse[0], errorResponse[0].ToString(), ex);
                    }
                }
                throw;
            }            
        }

        /// <summary>
        /// Post JSON for string content
        /// </summary>
        /// <param name="link"></param>
        /// <param name="requeststr"></param>
        /// <returns></returns>
        internal string PostJsonForStringContent<T>(string link, T requestobj, HttpParameters parameter = null)
        {
            try
            {

                // Make sure this is connected
                Connect();
                // Create cookie header
                Dictionary<string, string> header = new Dictionary<string, string>
                {
                    { "Token", AuthenticationToken.Token.DecryptedValue }
                };
                // Create request string
                string requeststr = JsonConvert.SerializeObject(requestobj, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = "yyyy-MM-dd"
                });
                // Post for result GUID
                string result = Proxy.SendJsonRequestForStringResult(link, Corely.Connections.HttpMethod.Post, requeststr, header, parameter);
                // Rreturn response
                return result;
            }
            catch (Exception ex)
            {
                if(ex.InnerException?.Message != null)
                {
                    // Thow exception of result can be parsed as an error object
                    List<ErrorResponse> errorResponse = JsonConvert.DeserializeObject<List<ErrorResponse>>(ex.InnerException.Message);
                    if (errorResponse != null) 
                    { 
                        throw new ErrorResponseException(errorResponse[0], errorResponse[0].ToString(), ex); 
                    }
                }
                throw;
            }
        }

        #endregion

    }
}
