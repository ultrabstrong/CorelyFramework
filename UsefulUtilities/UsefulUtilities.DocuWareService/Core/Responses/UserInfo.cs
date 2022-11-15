using UsefulUtilities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace UsefulUtilities.DocuWareService.Core.Responses
{
    [DataContract]
    public class UserInfo : ServiceResponseBase
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public bool IsOutOfOffice { get; set; }
    }
}