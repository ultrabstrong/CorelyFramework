using DocuWare.Platform.ServerClient;
using UsefulUtilities.Core;
using UsefulUtilities.Data.Serialization;
using UsefulUtilities.Extensions;
using UsefulUtilities.Logging;
using UsefulUtilities.Security;
using UsefulUtilities.Security.Authentication;
using UsefulUtilities.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using rm = UsefulUtilities.DocuWareService.Resources.SharedAndConfig;

namespace UsefulUtilities.DocuWareService
{
    public static class Shared
    {
        private static readonly string _progdatadir = $"C:/ProgramData/{(nameof(DocuWareService))}/";
        private static readonly string _logdir = $"C:/ProgramData/{(nameof(DocuWareService))}/logs/";
        private static readonly string _configfile = $"C:/ProgramData/{(nameof(DocuWareService))}/serviceconfig.xml";
        private static readonly string _apiKeyNotSet = "notset";

        /// <summary>
        /// Lock object for single-threaded interaction with configuration
        /// </summary>
        private static object ConfigLock { get; set; } = new object();

        #region Constructor / Initialization

        /// <summary>
        /// Initialize shard components the first time this is called
        /// </summary>
        static Shared()
        {
            // Create program data directory if it doesn't exist
            if (!Directory.Exists(_progdatadir))
            {
                Directory.CreateDirectory(_progdatadir);
            }
            // Initialize Logger
            Logger = new FileLogger(rm.serviceName, _logdir, rm.logFileName);
            try
            {
                // Load or create new service configuration
                if (!File.Exists(_configfile))
                {
                    // Create base configuration
                    Configuration = new ServiceConfig()
                    {
                        APIKey = _apiKeyNotSet,
                        LogLevel = LogLevel.WARN.GetLogLevelName(),
                        CredentialsKey = AESEncryption.CreateRandomBase64Key(),
                        TokenValidDays = 7
                    };
                    // Add base service credentials
                    Configuration.SvcCredentials = new GeneralCredentials(Configuration.CredentialsKey, "admin")
                    {
                        Username = "admin",
                        AuthenticationType = AuthenticationType.Basic,
                    };
                    // Save new configuraiton
                    SaveConfiguration();
                }
                else
                {
                    // Load configuration
                    LoadConfiguration();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(rm.failGetSvcConfig, ex, LogLevel.ERROR);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// File logger for writing to log files
        /// </summary>
        public static FileLogger Logger { get; set; }

        /// <summary>
        /// Configuration for this service
        /// </summary>
        public static ServiceConfig Configuration { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// Load current service configuration XML into memory
        /// </summary>
        public static void LoadConfiguration()
        {
            // Lock configuration to add new connection
            lock (ConfigLock)
            {
                // Deserialize configuration from log file
                Configuration = XmlSerializer.DeSerializeFromFile<ServiceConfig>(_configfile);
                // Set log level from configuration
                if (Configuration != null)
                {
                    try
                    {
                        // Set config log level if it is not already set
                        if (string.IsNullOrWhiteSpace(Configuration.LogLevel))
                        {
                            Configuration.LogLevel = LogLevel.WARN.GetLogLevelName();
                            SaveConfiguration();
                        }
                        // Set log level from config
                        Logger.LogLevel = Configuration.LogLevel.TryParseLogLevel();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog(rm.failSetLogLevel, ex, LogLevel.ERROR);
                    }
                }
            }
        }

        /// <summary>
        /// Save current configuration settings
        /// </summary>
        public static void SaveConfiguration()
        {
            lock (ConfigLock)
            {
                XmlSerializer.SerializeToFile(Configuration, _configfile);
            }
        }

        /// <summary>
        /// Create new DocuWare connection
        /// </summary>
        /// <param name="connectionName"></param>
        public static string CreateConnection(string connectionName)
        {
            // Lock configuration to add new connection
            lock (ConfigLock)
            {
                // Create unique id for connection
                string connectionId = Guid.NewGuid().ToString();
                while (Configuration.DWCredentials.Any(m => m.Id == connectionId)) { connectionId = Guid.NewGuid().ToString(); }
                // Create new credentials
                GeneralCredentials credentials = new GeneralCredentials(Configuration.CredentialsKey)
                {
                    Id = connectionId,
                    Name = connectionName,
                    Host = "https://localhost/DocuWare/Platform",
                    AuthenticationType = AuthenticationType.Basic,
                    Username = "dwadmin"
                };
                // Add and save new credentials
                Configuration.DWCredentials.Add(credentials);
                SaveConfiguration();
                return connectionId;
            }
        }

        /// <summary>
        /// Verify if API key is valid
        /// </summary>
        /// <param name="keyToCheck"></param>
        public static bool VerifyApiKey(string keyToCheck)
        {
#if !DEBUG
            if (string.IsNullOrWhiteSpace(Configuration.APIKey) || Configuration.APIKey == _apiKeyNotSet) { throw new Exception(rm.apiKeyNotSet); }
            if (Configuration.APIKey != keyToCheck) { throw new BadRequestException(BadRequestType.InvalidApiKey, rm.apiKeyInvalid); }
#endif
            return true;
        }

        /// <summary>
        /// Get connection list
        /// </summary>
        /// <returns></returns>
        public static NamedValues GetDWCredentialsList()
        {
            NamedValues namedValues = new NamedValues();
            if (Configuration?.DWCredentials?.Count > 0)
            {
                Configuration.DWCredentials.ForEach(m => namedValues.Add(m.Name, m.Id));
            }
            return namedValues;
        }

        /// <summary>
        /// Get DW credentials without AES values
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public static GeneralCredentials GetDWCredentialsNoEncryptedValues(string connectionId)
        {
            // Get credentials if they exist, else throw exception
            int index = Configuration?.DWCredentials?.FindIndex(m => m.Id == connectionId) ?? -1;
            if (index < 0) { throw new BadRequestException(BadRequestType.InvalidConnectionId, $"{rm.connNotFoundForId} [{connectionId}]"); }
            else
            {
                return Configuration.DWCredentials[index].GetWithoutAESValues();
            }
        }

        /// <summary>
        /// Set DW credentials without AES values
        /// </summary>
        /// <param name="credentials"></param>
        public static void SetDWCredentialsNoEncryptedValues(GeneralCredentials credentials)
        {
            // Get credentials if they exist, else throw exception
            int index = Configuration?.DWCredentials?.FindIndex(m => m.Id == credentials.Id) ?? -1;
            if (index < 0) { throw new BadRequestException(BadRequestType.InvalidConnectionId, $"{rm.connNotFoundForId} [{credentials.Id}]"); }
            else
            {
                // Update general credentials with new info
                lock (ConfigLock)
                {
                    Configuration.DWCredentials[index].Name = credentials.Name;
                    Configuration.DWCredentials[index].Host = credentials.Host;
                    Configuration.DWCredentials[index].Port = credentials.Port;
                    Configuration.DWCredentials[index].Domain = credentials.Domain;
                    Configuration.DWCredentials[index].Tenant = credentials.Tenant;
                    Configuration.DWCredentials[index].Username = credentials.Username;
                    SaveConfiguration();
                }
            }
        }

        /// <summary>
        /// Get service connection for connection id
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public static ServiceConnection GetServiceConnection(string connectionId)
        {
            // Get credentials if they exist, else throw exception
            int index = Configuration?.DWCredentials?.FindIndex(m => m.Id == connectionId) ?? -1;
            if (index < 0) { throw new BadRequestException(BadRequestType.InvalidConnectionId, $"{rm.connNotFoundForId} [{connectionId}]"); }
            else
            {
                ServiceConnection connection = null;
                try
                {
                    // Check if token is valid
                    if (Configuration.DWCredentials[index].Token?.IsValid() ?? false)
                    {
                        // Create connection with token
                        connection = ServiceConnection.Create(new Uri(Configuration.DWCredentials[index].Host), Configuration.DWCredentials[index].Token.Token.DecryptedValue);
                        // Re-up token if it expires in the next day
                        if (Configuration.DWCredentials[index].Token.IsValidForInterval(TimeSpan.FromDays(1)) == false)
                        {
                            // Update token for connection
                            UpdateToken(connection, index, TimeSpan.FromDays(Configuration.TokenValidDays));
                        }
                    }
                    else
                    {
                        // Create new connection with username and password
                        connection = ServiceConnection.Create(
                            new Uri(Configuration.DWCredentials[index].Host),
                            Configuration.DWCredentials[index].Username,
                            Configuration.DWCredentials[index].Password.DecryptedValue,
                            licenseType: DWProductTypes.PlatformService);
                        // Update token for connection
                        UpdateToken(connection, index, TimeSpan.FromDays(Configuration.TokenValidDays));
                    }
                    // Return connection
                    return connection;
                }
                catch
                {
                    // Terminate service connection if exception was caught
                    try { connection?.Disconnect(); } catch { }
                    throw;
                }
            }
        }

        /// <summary>
        /// Update token for credentials
        /// </summary>
        /// <param name="credentialsindex"></param>
        /// <param name="lifetime"></param>
        private static void UpdateToken(ServiceConnection connection, int credentialsindex, TimeSpan lifetime)
        {
            // Create new token
            string token = connection.Organizations[0].PostToLoginTokenRelationForString(new TokenDescription()
            {
                TargetProducts = new List<DWProductTypes> { DWProductTypes.PlatformService },
                Lifetime = lifetime.ToString(),
                Usage = TokenUsage.Multi
            });
            // Update token for connection
            lock (ConfigLock)
            {
                Configuration.DWCredentials[credentialsindex].Token = new AuthenticationToken(token, lifetime, Configuration.CredentialsKey);
                SaveConfiguration();
            }
        }

        #endregion
    }
}