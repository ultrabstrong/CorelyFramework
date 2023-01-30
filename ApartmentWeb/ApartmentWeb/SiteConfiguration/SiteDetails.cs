using BusinessLayer.Core;
using Corely.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApartmentWeb.SiteConfiguration
{
    public class SiteDetails
    {
        public string CompanyName { get; set; }

        public string CompanyShortName { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public bool ShowDownloadApplication { get; set; }

        public bool TenantInfoShowTrash { get; set; }

        public string TenantInfoPostOfficeAddress { get; set; }

        public NamedValues TenantInfoDocs { get; set; }

        public MailSettings MailSettings { get; set; }

    }
}