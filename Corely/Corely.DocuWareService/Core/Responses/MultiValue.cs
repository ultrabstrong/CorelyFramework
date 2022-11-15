using Corely.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Corely.DocuWareService.Core.Responses
{
    [DataContract]
    public class MultiValue : ServiceResponseBase
    {
        [DataMember]
        public List<string> Values { get; set; } = new List<string>();
    }
}