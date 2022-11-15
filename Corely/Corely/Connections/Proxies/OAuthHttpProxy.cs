using Corely.Security.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Connections.Proxies
{
    public class OAuthHttpProxy : HttpProxy
    {
        #region Constructors

        /// <summary>
        /// Must be constructed with credentials
        /// </summary>
        /// <param name="credentials"></param>
        public OAuthHttpProxy(OAuth1Credentials credentials)
        {
            Credentials = credentials;
            base.Connect(credentials.Host);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Credentials to authenticate with
        /// </summary>
        public OAuth1Credentials Credentials { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Disable the connection logic. Connection host comes from credentials host
        /// </summary>
        /// <param name="host"></param>
        public override void Connect(string host) { }

        /// <summary>
        /// Send request for HttpResponse result
        /// <para>This is the base request method that execute all other requests</para>
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <param name="headers"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override HttpResponseMessage SendRequestForHttpResponseNoErrors(string requestUri, HttpMethod method, HttpContent content, Dictionary<string, string> headers, HttpParameters parameters)
        {
            // Create authorization header
            OAuth1Credentials.OAuth1Authorization authString = Credentials.GetAuthorizationHeader(Connections.HttpMethod.Post, requestUri, parameters);
            // Add authorization header to headers list
            if(headers == null) { headers = new Dictionary<string, string>(); }
            headers.Add("Authorization", authString.Authorization);
            // Return response from base
            return base.SendRequestForHttpResponseNoErrors(requestUri, method, content, headers, parameters);
        }


        #endregion
    }
}
