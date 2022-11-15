using ApartmentWeb.SiteConfiguration;
using Corely.Data.Serialization;
using Corely.Extensions;
using Corely.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using rm = Resources.Website.Logs;

namespace ApartmentWeb
{ 
    public static class Shared
    {
        private static readonly string _datadir = $"{HttpContext.Current.Server.MapPath("~")}/data/";
        private static readonly string _logdir = $"{HttpContext.Current.Server.MapPath("~")}/logs/";
        private static readonly string _configfile = $"{HttpContext.Current.Server.MapPath("~")}/data/serviceconfig.xml";

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
            // Create data and log directories if they don't exist
            if (!Directory.Exists(_datadir))
            {
                Directory.CreateDirectory(_datadir);
            }
            if (!Directory.Exists(_logdir))
            {
                Directory.CreateDirectory(_logdir);
            }
            // Initialize Logger
            Logger = new FileLogger("WebApp", _logdir, "LogFile");
            Logger.FileLogManagementPolicy.DeleteDaysOld = 90;
            try
            {
                // Load or create new service configuration
                if (!File.Exists(_configfile))
                {
                    Configuration = new SiteConfig()
                    {
                        LogLevel = LogLevel.WARN.GetLogLevelName()
                    };
                    SaveConfiguration();
                }
                else
                {
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
        /// Configuration for this website
        /// </summary>
        public static SiteConfig Configuration { get; set; }

        /// <summary>
        /// Get current web version
        /// </summary>
        public static string Version => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        #endregion

        #region Methods

        /// <summary>
        /// Load current service configuration XML into memory
        /// </summary>
        public static void LoadConfiguration()
        {
            lock (ConfigLock)
            {
                // Deserialize configuration from log file
                Configuration = XmlSerializer.DeSerializeFromFile<SiteConfig>(_configfile);
                // Set log level from configuration
                if (Configuration != null)
                {
                    try
                    {
                        // Set log level if it is not already set
                        if (string.IsNullOrWhiteSpace(Configuration.LogLevel))
                        {
                            Configuration.LogLevel = LogLevel.WARN.ToString();
                            SaveConfiguration();
                        }
                        // Set log level
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

        #endregion
    }
}