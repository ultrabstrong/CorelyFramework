using UsefulUtilities.Data.Serialization;
using System;
using System.IO;
using System.Threading.Tasks;

namespace UsefulUtilities.Logging
{
    [Serializable]
    public class FileLogger : ILogger
    {
        #region Constructor

        /// <summary>
        /// Default constructor for deserialization. Use parameterized constructors
        /// </summary>
        public FileLogger()
        {
            FileLogManagementPolicy = new FileLogManagementPolicy(this);
        }

        /// <summary>
        /// Construct with logging path
        /// </summary>
        /// <param name="_logDirectory"></param>
        /// <param name="_logFile"></param>
        public FileLogger(string _sourceName, string _logDirectory, string _logFile) : this()
        {
            SourceName = _sourceName;
            LogDirectory = _logDirectory;
            LogFile = _logFile;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of code base where log is coming from
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// Log directory path
        /// </summary>
        public string LogDirectory { get; set; }

        /// <summary>
        /// Log file name
        /// </summary>
        public string LogFile { get; set; }

        /// <summary>
        /// Log file extension with dot included
        /// </summary>
        public string LogExtension { get; set; } = ".log";

        /// <summary>
        /// Level of logs to write
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.WARN;

        /// <summary>
        /// Lock object to help prevent multithreaded writing
        /// </summary>
        private object LockObject { get; set; } = new object();

        /// <summary>
        /// Management policy for rotating / deleting logs
        /// </summary>
        public FileLogManagementPolicy FileLogManagementPolicy { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// Write exception to log async
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="logLevel"></param>
        public async void WriteSerializedLogAsync<T>(string message, T toserialize, LogLevel logLevel) where T : class
        {
            // Don't log if log level isn't high enough
            if ((int)logLevel >= (int)this.LogLevel)
            {
                await Task.Run(() => WriteSerializedLog(message, toserialize, logLevel));
            }
        }

        /// <summary>
        /// Write serializeable object to log
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="toserialize"></param>
        /// <param name="logLevel"></param>
        public void WriteSerializedLog<T>(string message, T toserialize, LogLevel logLevel) where T : class
        {
            // Don't log if log level isn't high enough
            if ((int)logLevel >= (int)this.LogLevel)
            {
                if (typeof(T).IsSerializable)
                {
                    WriteLog(message, XmlSerializer.Serialize(toserialize), logLevel);
                }
                else
                {
                    WriteLog(message, toserialize.ToString(), logLevel);
                }
            }
        }

        /// <summary>
        /// Write exception to log async
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="logLevel"></param>
        public async void WriteLogAsync(string message, Exception ex, LogLevel logLevel)
        {
            // Don't log if log level isn't high enough
            if ((int)logLevel >= (int)this.LogLevel)
            {
                await Task.Run(() => WriteLog(message, ex, logLevel));
            }
        }

        /// <summary>
        /// Write exception to log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="logLevel"></param>
        public void WriteLog(string message, Exception ex, LogLevel logLevel)
        {
            // Don't log if log level isn't high enough
            if ((int)logLevel >= (int)this.LogLevel)
            {
                WriteSerializedLog(message, ex.ToString(), logLevel);
            }
        }

        /// <summary>
        /// Write data to log async
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <param name="logLevel"></param>
        public async void WriteLogAsync(string message, string data, LogLevel logLevel)
        {
            // Don't log if log level isn't high enough
            if ((int)logLevel >= (int)this.LogLevel)
            {
                await Task.Run(() => WriteSerializedLog(message, data, logLevel));
            }
        }

        /// <summary>
        /// Write data to log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <param name="logLevel"></param>
        public void WriteLog(string message, string data, LogLevel logLevel, int attempt = 1)
        {
            // Don't log if log level isn't high enough
            if ((int)logLevel >= (int)this.LogLevel)
            {
                try
                {
                    // Set this thread's culture
                    System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentUICulture;

                    // Create directory if it doesn't exist
                    if (!Directory.Exists(LogDirectory))
                    {
                        Directory.CreateDirectory(LogDirectory);
                    }

                    // Get rotated file name for policy settings
                    string filepath = FileLogManagementPolicy.GetRotatedLogFilePath();

                    // Build output 
                    string output = $"{logLevel} [{SourceName}]: {message} .      {DateTime.Now}";
                    if (string.IsNullOrWhiteSpace(data)) { output += $"{Environment.NewLine}{Environment.NewLine}"; }
                    else { output += $"{Environment.NewLine}{data}{Environment.NewLine}{Environment.NewLine}"; }

                    // Output message to file
                    lock (LockObject)
                    {
                        File.AppendAllText(filepath, output);
                    }
                }
                catch (Exception)
                {
                    // Sleep and try again up to nine more times
                    System.Threading.Thread.Sleep(100);
                    if (attempt < 10)
                    {
                        WriteLog(message, data, logLevel, ++attempt);
                    }
                }
                finally
                {
                    try
                    {
                        FileLogManagementPolicy.RunDeletionPolicy();
                    }
                    catch (Exception)
                    {
                        // Do nothing
                    }
                }
            }
        }

        #endregion

    }
}
