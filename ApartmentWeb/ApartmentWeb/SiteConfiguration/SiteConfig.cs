using BusinessLayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Xml.Serialization;

namespace ApartmentWeb.SiteConfiguration
{
    [Serializable]
    public class SiteConfig
    {
        /// <summary>
        /// Site details
        /// </summary>
        public SiteDetails SiteDetails { get; set; }

        /// <summary>
        /// Log level
        /// </summary>
        public string LogLevel { get; set; }
        
    }
}