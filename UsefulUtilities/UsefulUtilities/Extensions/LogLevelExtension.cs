using UsefulUtilities.Logging;
using rm = UsefulUtilities.Resources.Logging.LogLevel;

namespace UsefulUtilities.Extensions
{
    public static class LogLevelExtension
    {
        /// <summary>
        /// Get proper name for log level
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public static string GetLogLevelName(this Logging.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.DEBUG: return rm.DEBUG;
                case LogLevel.NOTICE: return rm.NOTICE;
                case LogLevel.WARN: return rm.WARN;
                case LogLevel.ERROR:
                default: return rm.ERROR;
            }
        }

        /// <summary>
        /// Convert string log level to log level with normalized conversion
        /// </summary>
        /// <param name="logLevelStr"></param>
        /// <returns></returns>
        public static LogLevel TryParseLogLevel(this string logLevelStr)
        {
            LogLevel logLevel = LogLevel.WARN;
            if (logLevelStr.ToLower().Contains(LogLevel.ERROR.GetLogLevelName().ToLower()))
            {
                logLevel = LogLevel.ERROR;
            }
            else if (logLevelStr.ToLower().Contains(LogLevel.WARN.GetLogLevelName().ToLower()))
            {
                logLevel = LogLevel.WARN;
            }
            else if (logLevelStr.ToLower().Contains(LogLevel.NOTICE.GetLogLevelName().ToLower()))
            {
                logLevel = LogLevel.NOTICE;
            }
            else if (logLevelStr.ToLower().Contains(LogLevel.DEBUG.GetLogLevelName().ToLower()))
            {
                logLevel = LogLevel.DEBUG;
            }
            return logLevel;
        }
    }
}
