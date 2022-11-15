using UsefulUtilities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace UsefulUtilities.LibPostalService
{
    public class LibPostalServiceResponse : ServiceResponseBase
    {
        public List<AddressPart> ParsedAddress { get; set; }
    }
}