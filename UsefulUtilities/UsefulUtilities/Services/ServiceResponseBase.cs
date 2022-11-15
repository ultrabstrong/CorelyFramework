using System;
using System.Net;
using System.Runtime.Serialization;

namespace UsefulUtilities.Services
{
    [DataContract]
    public class ServiceResponseBase : IServiceResponse
    {
        #region Constructor

        public ServiceResponseBase() { }

        #endregion

        #region Properties

        [DataMember]
        public int Status { get; set; } = (int)HttpStatusCode.OK;

        [DataMember]
        public string Message { get; set; } = HttpStatusCode.OK.ToString();

        [DataMember]
        public string Data { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Set basic response
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public void SetOKMessage(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Set basic response
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public void SetWithStatus(string message, HttpStatusCode status)
        {
            Message = message;
            Status = (int)status;
        }

        /// <summary>
        /// Set response with data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <param name="status"></param>
        public void SetWithData(string message, string data, HttpStatusCode status)
        {
            Message = message;
            Data = data;
            Status = (int)status;
        }

        /// <summary>
        /// Set response for exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="status"></param>
        public void SetWithException(string message, Exception ex, HttpStatusCode status)
        {
            SetWithData(message, ex.ToString(), status);
        }

        /// <summary>
        /// Return status and message
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{Status}] - {Message}";
        }

        #endregion
    }
}
