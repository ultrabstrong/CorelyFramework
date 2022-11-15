using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Core
{
    public class ResultBase
    {

        #region Properties

        /// <summary>
        /// Success flag
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Result message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Result data
        /// </summary>
        private List<string> Data { get; set; } = new List<string>();

        /// <summary>
        /// Result exception
        /// </summary>
        public Exception Exception 
        { 
            get => _exception;
            set
            {
                _exception = value;
                Succeeded = false;
            }
        }
        private Exception _exception;

        #endregion

        #region Methods

        /// <summary>
        /// Set or add message
        /// </summary>
        public void AddMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(Message)) { Message = message; }
            else { Message += $"{Environment.NewLine}{message}"; }
        }

        /// <summary>
        /// Add data and flag error
        /// </summary>
        /// <param name="data"></param>
        public void AddDataError(string data)
        {
            Succeeded = false;
            AddData(data);
        }

        /// <summary>
        /// Add data
        /// </summary>
        /// <param name="error"></param>
        public void AddData(string data)
        {
            Data.Add(data);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        public List<string> GetData => Data;

        /// <summary>
        /// Get data list as string
        /// </summary>
        /// <returns></returns>
        public string GetDataString()
        {
            if(Data?.Count == 0) { return ""; }
            return string.Join(Environment.NewLine, Data);
        }

        /// <summary>
        /// Get display errors as string
        /// </summary>
        /// <returns></returns>
        public string GetErrorsDisplayString()
        {
            if (Succeeded == false)
            {
                return Message + Environment.NewLine + GetDataString();
            }
            else if (Data?.Count > 0)
            {
                return GetDataString();
            }
            return "";
        }

        /// <summary>
        /// Display errors in message box
        /// </summary>
        public void DisplayErrors()
        {
            string errors = GetErrorsDisplayString();
            if (!string.IsNullOrWhiteSpace(errors))
            {
                Core.Message.ShowAsync(errors);
            }
        }

        /// <summary>
        /// Get stringified properties
        /// </summary>
        /// <returns></returns>
        public string GetResultString()
        {
            string result = Message;
            // Add data to result
            if (Data?.Count > 0)
            {
                result += Environment.NewLine + GetDataString();
            }
            // Add error to result
            if (Exception != null)
            {
                result += Environment.NewLine + Exception.ToString();
            }
            return result;
        }

        #endregion
    }
}
