using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using rm = UsefulUtilities.Resources.Connections.Proxies.HttpProxy;

namespace UsefulUtilities.Connections.Proxies
{
    public class HttpProxy : IDisposable
    {
        #region Constructor / Initialization

        /// <summary>
        /// Base constructor
        /// </summary>
        public HttpProxy() { }

        /// <summary>
        /// Construct with host
        /// </summary>
        public HttpProxy(string host) 
        {
            Connect(host);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Http client for making requests
        /// </summary>
        private HttpClient HttpClient { get; set; }

        /// <summary>
        /// Is there a connection to the http proxy
        /// </summary>
        public bool IsConnected { get; internal set; } = false;

        #endregion

        #region Connection

        /// <summary>
        /// Connect to http client
        /// </summary>
        public virtual void Connect(string host)
        {
            if (!IsConnected)
            {
                try
                {
                    // Create HttPClient with base address
                    HttpClient = new HttpClient() { BaseAddress = new Uri(host) };
                    // Set connected to true
                    IsConnected = true;
                }
                catch
                {
                    IsConnected = false;
                    throw;
                }
            }
        }

        /// <summary>
        /// Disconnect http connection
        /// </summary>
        public virtual void Disconnect()
        {
            if (IsConnected)
            {
                IsConnected = false;
                HttpClient?.Dispose();
                HttpClient = null;
            }
        }

        #endregion

        #region Base Requests / Helpers

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
        public virtual HttpResponseMessage SendRequestForHttpResponseNoErrors(string requestUri, HttpMethod method, HttpContent content, Dictionary<string, string> headers, HttpParameters parameters)
        {
            // Check for connection
            if (!IsConnected)
            {
                throw new Exception(rm.proxyNotConnected);
            }
            // Build parameterized request uri if provided
            if (parameters != null && (parameters.HasParameters() || parameters.HasTempParameters()))
            {
                requestUri += $"?{parameters.GetParamString()}";
            }
            // Build generic Http request
            HttpRequestMessage message = new HttpRequestMessage(GetSystemHttpMethodForCustomHttpMethod(method), requestUri);
            // Build http headers if provided
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }
            // Build content if provided
            if (content != null)
            {
                message.Content = content;
            }
            // Send request for result
            HttpResponseMessage result = HttpClient.SendAsync(message).GetAwaiter().GetResult();
            // Return result
            return result;
        }

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
        public virtual HttpResponseMessage SendRequestForHttpResponse(string requestUri, HttpMethod method, HttpContent content, Dictionary<string, string> headers, HttpParameters parameters)
        {
            // Send request for result
            HttpResponseMessage result = SendRequestForHttpResponseNoErrors(requestUri, method, content, headers, parameters);
            // Handle response
            if (result.IsSuccessStatusCode)
            {
                // Return for successful result
                return result;
            }
            else
            {
                // Throw error for failed result
                string body = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                throw new HttpRequestException($"{(int)result.StatusCode} {result.StatusCode} - {result.ReasonPhrase}{Environment.NewLine}{result.RequestMessage}", new Exception(body));
            }
        }



        /// <summary>
        /// Send request for string result
        /// <para>Base request method for sending with string response</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <param name="headers"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual string SendRequestForStringResult(string requestUri, HttpMethod method, HttpContent content, Dictionary<string, string> headers = null, HttpParameters parameters = null)
        {
            HttpResponseMessage result = SendRequestForHttpResponse(requestUri, method, content, headers, parameters);
            return result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Execute Multipart form HTTP request for string result
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="method"></param>
        /// <param name="formdata"></param>
        /// <param name="headers"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual string SendFormRequestForStringResult(string requestUri, HttpMethod method, Dictionary<string, string> formdata, Dictionary<string, string> headers = null, HttpParameters parameters = null)
        {
            // Build content
            HttpContent content = BuildFormRequestContent(formdata);
            // Post request
            return SendRequestForStringResult(requestUri, method, content, headers, parameters);
        }

        /// <summary>
        /// Execute Url encoded HTTP request for string result
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="formdata"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public virtual string SendUrlencodedRequestForStringResult(string requestUri, HttpMethod method, Dictionary<string, string> formdata, Dictionary<string, string> headers = null, HttpParameters parameters = null)
        {
            // Build content
            HttpContent content = BuildUrlencodedContent(formdata);
            // Post request
            return SendRequestForStringResult(requestUri, method, content, headers, parameters);
        }

        /// <summary>
        /// Execute Json HTTP request for string result
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="jsonContent"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public virtual string SendJsonRequestForStringResult(string requestUri, HttpMethod method, string jsonContent, Dictionary<string, string> headers = null, HttpParameters parameters = null)
        {
            // Build content
            HttpContent content = BuildJsonContent(jsonContent);
            // Send request and return response
            return SendRequestForStringResult(requestUri, method, content, headers, parameters);
        }




        /// <summary>
        /// Send HTTP request
        /// <para>Base request method for sending with no response</para>
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public virtual void SendRequest(string requestUri, HttpMethod method, HttpContent content, Dictionary<string, string> headers = null, HttpParameters parameters = null)
        {
            SendRequestForHttpResponse(requestUri, method, content, headers, parameters);
        }

        /// <summary>
        /// Execute Multipart form HTTP request
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="formdata"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public virtual void SendFormRequest(string requestUri, HttpMethod method, Dictionary<string, string> formdata, Dictionary<string, string> headers = null, HttpParameters parameters = null)
        {
            // Build content
            HttpContent content = BuildFormRequestContent(formdata);
            // Post request
            SendRequest(requestUri, method, content, headers, parameters);
        }

        /// <summary>
        /// Execute Url encoded HTTP request
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="formdata"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public virtual void SendUrlencodedRequest(string requestUri, HttpMethod method, Dictionary<string, string> formdata, Dictionary<string, string> headers = null, HttpParameters parameters = null)
        {
            // Build content
            HttpContent content = BuildUrlencodedContent(formdata);
            // Post request
            SendRequest(requestUri, method, content, headers, parameters);
        }

        /// <summary>
        /// Execute Json HTTP request
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="jsonContent"></param>
        /// <param name="headers"></param>
        public virtual void SendJsonRequest(string requestUri, HttpMethod method, string jsonContent, Dictionary<string, string> headers = null, HttpParameters parameters = null)
        {
            // Build content
            HttpContent content = BuildJsonContent(jsonContent);
            // Send request
            SendRequest(requestUri, method, content, headers, parameters);
        }

        #endregion

        #region Content Helpers / Other Helpers

        /// <summary>
        /// Build Multipart Form Content
        /// </summary>
        /// <param name="formdata"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public virtual HttpContent BuildFormRequestContent(Dictionary<string, string> formdata)
        {
            // Build request content
            MultipartFormDataContent content = new MultipartFormDataContent();
            // Add form data
            if (formdata != null)
            {
                foreach (KeyValuePair<string, string> formvals in formdata)
                {
                    content.Add(new StringContent(formvals.Value), formvals.Key);
                }
            }
            // Return content
            return content;
        }

        /// <summary>
        /// Build Url Encoded Form Content
        /// </summary>
        /// <param name="formdata"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public virtual HttpContent BuildUrlencodedContent(Dictionary<string, string> formdata)
        {
            // Build request content
            FormUrlEncodedContent content = new FormUrlEncodedContent(formdata);
            // Return content
            return content;
        }

        /// <summary>
        /// Build Json content
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public virtual HttpContent BuildJsonContent(string jsonContent)
        {
            // Build request content
            HttpContent content = new StringContent(jsonContent);
            // Add headers
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // Return content
            return content;
        }

        /// <summary>
        /// Convert custom http method to system http method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private System.Net.Http.HttpMethod GetSystemHttpMethodForCustomHttpMethod(HttpMethod method)
        {
            switch (method)
            {
                case HttpMethod.Delete:
                    return System.Net.Http.HttpMethod.Delete;
                case HttpMethod.Put:
                    return System.Net.Http.HttpMethod.Put;
                case HttpMethod.Post:
                    return System.Net.Http.HttpMethod.Post;
                case HttpMethod.Get:
                default:
                    return System.Net.Http.HttpMethod.Get;
            }
        }

        #endregion

        #region Dispose

        private bool _disposed = false;

        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose this object only once
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try { Disconnect(); }
                    catch { }
                }
                _disposed = true;
            }
        }

        #endregion
    }
}
