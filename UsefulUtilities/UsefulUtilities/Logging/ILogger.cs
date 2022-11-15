using System;

namespace UsefulUtilities.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Write serializeable object to log async
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="logLevel"></param>
        void WriteSerializedLogAsync<T>(string message, T toserialize, LogLevel logLevel) where T : class;

        /// <summary>
        /// Write serializeable object to log
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="toserialize"></param>
        /// <param name="logLevel"></param>
        void WriteSerializedLog<T>(string message, T toserialize, LogLevel logLevel) where T : class;

        /// <summary>
        /// Write exception to log async
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="logLevel"></param>
        void WriteLogAsync(string message, Exception ex, LogLevel logLevel);

        /// <summary>
        /// Write exception to log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="logLevel"></param>
        void WriteLog(string message, Exception ex, LogLevel logLevel);

        /// <summary>
        /// Write data to log async
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <param name="logLevel"></param>
        void WriteLogAsync(string message, string data, LogLevel logLevel);

        /// <summary>
        /// Write data to log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <param name="logLevel"></param>
        void WriteLog(string message, string data, LogLevel logLevel, int attempt = 1);
    }
}
