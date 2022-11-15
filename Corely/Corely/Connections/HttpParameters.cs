using Corely.Core;
using Corely.Data.Encoding;
using System.Collections.Generic;

namespace Corely.Connections
{
    public class HttpParameters
    {
        #region Constructor

        /// <summary>
        /// Empty constructor
        /// </summary>
        public HttpParameters() 
        { 
            Parameters = new Dictionary<string, string>();
            TempParameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Create http parameter
        /// </summary>
        /// <param name="paramname"></param>
        /// <param name="paramvalue"></param>
        public HttpParameters(string key, string value)
        {
            Parameters = new Dictionary<string, string>() { { key, value } };
            TempParameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Create http parameters with multiple params
        /// </summary>
        /// <param name="paramname"></param>
        /// <param name="paramvalue"></param>
        public HttpParameters(Dictionary<string, string> parameters)
        {
            Parameters = parameters;
            TempParameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Create http parameters with multiple params and temp params
        /// </summary>
        /// <param name="paramname"></param>
        /// <param name="paramvalue"></param>
        public HttpParameters(Dictionary<string, string> parameters, Dictionary<string, string> tempParameters) 
        {
            Parameters = parameters;
            TempParameters = tempParameters;
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Parameters list
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// Parameters list that is nullified after every GetParamString call
        /// </summary>
        public Dictionary<string, string> TempParameters { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// ? Do parameters exist
        /// </summary>
        /// <returns></returns>
        public bool HasParameters() => Parameters?.Count > 0;

        /// <summary>
        /// ? Do temp parameters exist
        /// </summary>
        /// <returns></returns>
        public bool HasTempParameters() => TempParameters?.Count > 0;

        /// <summary>
        /// Return parameter string
        /// </summary>
        /// <returns></returns>
        public string GetParamString()
        {
            string param = "";
            bool first = true;
            // Check if params were provided
            if (Parameters != null && Parameters.Keys.Count > 0)
            {
                // Iterate parameter
                foreach (string key in Parameters.Keys)
                {
                    string value = Parameters[key];
                    // Validate key and value
                    if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
                    {
                        // Append ampersand for multiple params
                        if (first == false)
                        {
                            param += "&";
                        }
                        first = false;
                        // Add url encoded param value
                        param += $"{key}={value.UrlEncode()}";

                    }
                }
            }
            // Check if temp params exist
            if (TempParameters != null && TempParameters.Keys.Count > 0)
            {
                // Iterate parameter
                foreach (string key in TempParameters.Keys)
                {
                    string value = TempParameters[key];
                    // Validate key and value
                    if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
                    {
                        // Append ampersand for multiple params
                        if (first == false)
                        {
                            param += "&";
                        }
                        first = false;
                        // Add url encoded param value
                        param += $"{key}={value.UrlEncode()}";

                    }
                }
                // Nullify temp params
                TempParameters = null;
            }
            return param;
        }

        /// <summary>
        /// Return HTTP Parameters as named values
        /// </summary>
        /// <returns></returns>
        public NamedValues ToNamedValues()
        {
            // Return null if there are no parameters
            if (Parameters == null && TempParameters == null) { return null; }
            NamedValues values = new NamedValues();
            // Add parameters to values
            if (Parameters != null)
            {
                foreach (KeyValuePair<string, string> kvp in Parameters)
                {
                    values.Add(kvp.Key, kvp.Value);
                }
            }
            // Add temp parameters to values
            if (TempParameters != null)
            {
                foreach (KeyValuePair<string, string> kvp in TempParameters)
                {
                    values.Add(kvp.Key, kvp.Value);
                }
            }
            return values;
        }

        #endregion
    }
}
