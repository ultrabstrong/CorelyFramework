using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Services
{
    public interface IServiceResponse
    {
        void SetOKMessage(string message);

        void SetWithStatus(string message, HttpStatusCode status);

        void SetWithData(string message, string data, HttpStatusCode status);

        void SetWithException(string message, Exception ex, HttpStatusCode status);
    }
}
