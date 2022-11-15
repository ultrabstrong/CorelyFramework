using KelleyUtilities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KelleyUtilities.LibPostalService
{
    [Serializable]
    public class ServiceResponse : ServiceResponseBase
    {
        public List<KeyValuePair<string, string>> ParsedAddress { get; set; }
    }
}