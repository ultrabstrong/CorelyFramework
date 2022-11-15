using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApartmentWeb.SiteConfiguration
{
    public static class SiteDetailsFactory
    {
        public static ISiteDetails GetSiteDetails(int id)
        {
            switch (id)
            {
                case 1:
                    return new ApexPropertiesSiteDetails();
                case 2:
                    return new RentInBozemanSiteDetails();
                default:
                    return null;
            }
        }
    }
}