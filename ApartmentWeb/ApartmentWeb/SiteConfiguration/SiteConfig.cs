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
        [XmlIgnore]
        public ISiteDetails SiteDetails { get; internal set; }

        /// <summary>
        /// Site details ID
        /// </summary>
        public int SiteDetailsId 
        { 
            get => _siteDetailsId;
            set
            {
                _siteDetailsId = value;
                ISiteDetails siteDetails = SiteDetailsFactory.GetSiteDetails(_siteDetailsId);
                SiteDetails = siteDetails;
            }
        }
        private int _siteDetailsId;

        /// <summary>
        /// Log level
        /// </summary>
        public string LogLevel { get; set; }
        
    }
}