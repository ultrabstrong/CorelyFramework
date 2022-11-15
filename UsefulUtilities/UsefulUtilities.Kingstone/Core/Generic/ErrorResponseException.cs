using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Kingstone.Core
{
    public class ErrorResponseException : Exception
    {
        public ErrorResponseException(ErrorResponse errorResponse) : base() 
        {
            ErrorResponse = errorResponse;
        }

        public ErrorResponseException(ErrorResponse errorResponse, string message) : base(message)
        {
            ErrorResponse = errorResponse;
        }

        public ErrorResponseException(ErrorResponse errorResponse, string message, Exception innerException) : base (message, innerException)
        {
            ErrorResponse = errorResponse;
        }

        public ErrorResponse ErrorResponse { get; internal set; }
    }
}
