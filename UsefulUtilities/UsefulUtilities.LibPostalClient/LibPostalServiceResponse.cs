using UsefulUtilities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.LibPostalClient
{
    [Serializable]
    public class LibPostalServiceResponse : ServiceResponseBase
    {
        public List<AddressPart> ParsedAddress { get; set; }
    }
}
