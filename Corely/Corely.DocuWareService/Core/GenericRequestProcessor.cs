using DocuWare.Platform.ServerClient;
using Corely.Logging;
using Corely.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using rm = Corely.DocuWareService.Resources.SharedAndConfig;

namespace Corely.DocuWareService.Core
{
    public static class GenericRequestProcessor
    {
        /// <summary>
        /// Check key and process action
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="errormessage"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T Process<T>(string apikey, string errormessage, Action<T> action, params (string name, object value)[] paramsToVerify) where T : IServiceResponse, new()
        {
            // Initialize response
            T response = new T();
            try
            {
                // Verify api key
                Shared.VerifyApiKey(apikey);
                // Check if there are parameters to verify
                if(paramsToVerify != null && paramsToVerify.Length > 0)
                {
                    // Get names of parameters with empty values
                    List<string> badParams = paramsToVerify.ToList().Where(m => string.IsNullOrWhiteSpace(m.value?.ToString())).Select(m => m.name).ToList();
                    if(badParams.Count > 0)
                    {
                        throw new BadRequestException(BadRequestType.InvalidParameters, $"{rm.paramsNotEmpty}: {string.Join(",", badParams)}");
                    }                
                }
                // Perform action
                action?.Invoke(response);
            }
            catch (BadRequestException ex)
            {
                // Handle bad request exception as client error
                response.SetWithException($"[{ex.Type}] {ex.Message}", ex, HttpStatusCode.BadRequest);
                Shared.Logger.WriteLog($"[{ex.Type}] {ex.Message}", ex, LogLevel.NOTICE);
            }
            catch (Exception ex)
            {
                // Handle exception as service error
                response.SetWithException(errormessage, ex, HttpStatusCode.InternalServerError);
                Shared.Logger.WriteLog(errormessage, ex, LogLevel.ERROR);
            }
            return response;
        }

        /// <summary>
        /// Check key and process action
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="errormessage"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T ConnectAndProcess<T>(string apikey, string connectionid, string errormessage, Action<T, ServiceConnection> action, params (string name, object value)[] paramsToVerify) where T : IServiceResponse, new()
        {
            // Perform basic processing action
            return Process<T>(apikey, errormessage, response =>
            {
                ServiceConnection connection = null;
                try
                {
                    // Create connection
                    connection = Shared.GetServiceConnection(connectionid);
                    // Perform action
                    action?.Invoke(response, connection);
                }
                finally
                {
                    // Terminate service connection
                    try { connection?.Disconnect(); } catch { }
                }
            }, paramsToVerify);
        }
    }
}