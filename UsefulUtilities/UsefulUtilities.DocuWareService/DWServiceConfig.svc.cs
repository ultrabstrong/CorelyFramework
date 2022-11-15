using UsefulUtilities.Core;
using UsefulUtilities.Data.Serialization;
using UsefulUtilities.DocuWareService.Core;
using UsefulUtilities.Logging;
using UsefulUtilities.Security;
using UsefulUtilities.Security.Authentication;
using UsefulUtilities.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using rm = UsefulUtilities.DocuWareService.Resources.SharedAndConfig;

namespace UsefulUtilities.DocuWareService
{
    public class DWServiceConfig : IDWServiceConfig
    {
        /// <summary>
        /// Create connection
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="connectionnname"></param>
        /// <returns></returns>
        public ServiceResponseBase CreateConnection(string apikey, string connectionnname)
        {
            return GenericRequestProcessor.Process<ServiceResponseBase>(apikey, rm.failCreateConnection, response =>
            {
                string connectionid = Shared.CreateConnection(connectionnname);
                response.SetOKMessage(connectionid);
            }, (nameof(connectionnname), connectionnname));
        }

        /// <summary>
        /// Create or update connection credentials with password provided
        /// </summary>
        /// <param name="oldpw"></param>
        /// <param name="newpw"></param>
        /// <param name="confirmnewpw"></param>
        /// <returns></returns>
        public ServiceResponseBase UpdatePW(string apikey, string update, string oldpw, string newpw, string confirmpw)
        {
            return GenericRequestProcessor.Process<ServiceResponseBase>(apikey, rm.failUpdatePW, response =>
            {
                // Validate new password
                if (newpw != confirmpw)
                {
                    response.SetWithStatus($"{HttpStatusCode.BadRequest} {rm.passwordsDontMatch}", HttpStatusCode.BadRequest);
                }
                else
                {
                    // Check which credential set to update
                    if (update.ToLower().Contains("svc"))
                    {
                        // Find and update Svc credentials
                        if (Shared.Configuration.SvcCredentials == null)
                        {
                            response.SetWithStatus($"{HttpStatusCode.NotFound} {rm.svcCredentialsNotFound}", HttpStatusCode.BadRequest);
                        }
                        else
                        {
                            // Verify saved credentials password is the same as the old password provided
                            if (!string.IsNullOrWhiteSpace(Shared.Configuration.SvcCredentials.Password.DecryptedValue) &&
                                Shared.Configuration.SvcCredentials.Password.DecryptedValue != oldpw)
                            {
                                // Return bad request response if passwords don't match
                                response.SetWithStatus($"{HttpStatusCode.BadRequest} {rm.oldPasswordWrong}", HttpStatusCode.BadRequest);
                            }
                            else
                            {
                                // Update password if old password does match
                                Shared.Configuration.SvcCredentials.Password.DecryptedValue = newpw;
                            }
                        }
                    }
                    else if (Guid.TryParse(update, out Guid updateid))
                    {
                        // Find and update DW credentials
                        int index = Shared.Configuration.DWCredentials.FindIndex(m => m.Id == update);
                        if (index < 0)
                        {
                            response.SetWithStatus($"{HttpStatusCode.NotFound} {rm.connNotFoundForId} {update}", HttpStatusCode.NotFound);
                        }
                        else
                        {
                            // Verify saved credentials password is the same as the old password provided
                            if (!string.IsNullOrWhiteSpace(Shared.Configuration.DWCredentials[index].Password.DecryptedValue) &&
                                Shared.Configuration.DWCredentials[index].Password.DecryptedValue != oldpw)
                            {
                                // Return bad request response if passwords don't match
                                response.SetWithStatus($"{HttpStatusCode.BadRequest} {rm.oldPasswordWrong}", HttpStatusCode.BadRequest);
                            }
                            else
                            {
                                // Update password if old password does match
                                Shared.Configuration.DWCredentials[index].Password.DecryptedValue = newpw;
                            }
                        }
                    }
                    else
                    {
                        response.SetWithStatus($"{HttpStatusCode.BadRequest} {rm.updateParamWrong}", HttpStatusCode.BadRequest);
                    }

                    // Only proceed if credentials were set correctly
                    if (response.Status != (int)HttpStatusCode.BadRequest)
                    {
                        // Save configuration with updated credentials
                        Shared.SaveConfiguration();
                        Shared.Logger.WriteLog(rm.passwordUpdatedSuccessfully, "", LogLevel.NOTICE);
                        response.SetOKMessage(rm.passwordUpdatedSuccessfully);
                    }
                }
            });
        }

        /// <summary>
        /// Get assembly version of this
        /// </summary>
        /// <returns></returns>
        public string GetVersion() => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        /// <summary>
        /// Reload service configuration from XML
        /// </summary>
        /// <returns></returns>
        public ServiceResponseBase ReloadServiceConfig(string apikey)
        {
            return GenericRequestProcessor.Process<ServiceResponseBase>(apikey, rm.failReloadServiceConfig, response =>
            {
                Shared.LoadConfiguration();
            });
        }

        /// <summary>
        /// Test DW connection
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="connectionid"></param>
        /// <returns></returns>
        public ServiceResponseBase TestDWConnection(string apikey, string connectionid)
        {
            return GenericRequestProcessor.ConnectAndProcess<ServiceResponseBase>(apikey, connectionid, rm.failTestConnection, (response, connection) =>
            {
                response.Data = connection.ServiceDescription.Version;
            });
        }

        /// <summary>
        /// Get DW credentials list
        /// </summary>
        /// <param name="apikey"></param>
        /// <returns></returns>
        public ServiceResponseBase GetDWCredentialsList(string apikey)
        {
            return GenericRequestProcessor.Process<ServiceResponseBase>(apikey, rm.failGetDWCredentialsList, response =>
            {
                NamedValues namedValues = Shared.GetDWCredentialsList();
                response.Data = XmlSerializer.Serialize(namedValues);
            });
        }

        /// <summary>
        /// Get DW credentials for connection id
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="connectionid"></param>
        /// <returns></returns>
        public ServiceResponseBase GetDWCredentials(string apikey, string connectionid)
        {
            return GenericRequestProcessor.Process<ServiceResponseBase>(apikey, rm.failGetDWCredentials, response =>
            {
                GeneralCredentials credentials = Shared.GetDWCredentialsNoEncryptedValues(connectionid);
                response.Data = XmlSerializer.Serialize(credentials);
            }, (nameof(connectionid), connectionid));
        }

        /// <summary>
        /// Set DW credentials with credentials xml
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="credentialsxml"></param>
        /// <returns></returns>
        public ServiceResponseBase SetDWCredentials(string apikey, string credentialsxml)
        {
            return GenericRequestProcessor.Process<ServiceResponseBase>(apikey, rm.failSetDWCredentials, response =>
            {
                GeneralCredentials credentials = XmlSerializer.DeSerialize<GeneralCredentials>(credentialsxml);
                if(credentials == null) { throw new BadRequestException(BadRequestType.InvalidParameters, rm.invalidXml); }
                Shared.SetDWCredentialsNoEncryptedValues(credentials);
            }, (nameof(credentialsxml), credentialsxml));
        }
    }
}
