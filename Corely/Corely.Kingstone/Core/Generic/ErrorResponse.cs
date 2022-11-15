using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Kingstone.Core
{
    [Serializable]
    public class ErrorResponse
    {
        #region Properties 

        public string Status { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public List<ErrorResponse> Messages { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Return string of this error and errors from messages
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string error = $"[{Date:yyyy-MM-dd HH:mm:ss}] {Status} - {Description}";
            if(Messages != null && Messages.Count > 0)
            {
                string additionalErrors = string.Join(Environment.NewLine, Messages.Select(m => m.ToString()));
                error = $"{error}{Environment.NewLine}{additionalErrors}";
            }
            return error;
        }

        #endregion
    }
}
