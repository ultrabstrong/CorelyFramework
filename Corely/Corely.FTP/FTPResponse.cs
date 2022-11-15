using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.FTP
{
    [Serializable]
    public class FTPResponse : Corely.Services.ServiceResponseBase
    {
        #region Properties

        /// <summary>
        /// Exception for failures
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Data { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Set exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="status"></param>
        public void SetWithException(string message, Exception ex, System.Net.HttpStatusCode status)
        {
            base.SetWithException(message, ex, status);
            Exception = ex;
        }

        #endregion
    }
}
