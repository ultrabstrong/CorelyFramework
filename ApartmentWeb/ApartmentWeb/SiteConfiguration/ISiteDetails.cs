using BusinessLayer.Core;
using Corely.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApartmentWeb.SiteConfiguration
{
    public interface ISiteDetails
    {
        string CompanyName { get; }

        string CompanyShortName { get; }

        string EmailAddress { get; }

        string PhoneNumber { get; }

        string Address { get; }

        bool ShowTrash { get; }

        bool ShowDownloadApplication { get; }

        string PostOfficeAddress { get; }

        NamedValues TenantInfoDocs { get; }

        MailSettings MailSettings { get; }

    }
}