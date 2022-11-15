using Corely.Connections;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Corely.Sage300HH2.Core
{
    [Serializable]
    public class Job : IVersioned
    {
        #region Properties

        public string Id { get; set; }

        public int Version { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Address4 { get; set; }

        public string BillingLevel { get; set; }

        public string BillingMethod { get; set; }

        public string City { get; set; }

        public string Code { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public bool HasExternalId { get; set; }

        public bool IsActive { get; set; }

        public bool IsArchived { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string Name { get; set; }

        public string ParentId { get; set; }

        public string PostalCode { get; set; }

        public bool ShouldUseProjectManagement { get; set; }

        public string State { get; set; }

        public string Status { get; set; }

        public string ParentCode { get; set; }

        public string ParentName { get; set; }

        public decimal Misc1Amount { get; set; }

        public decimal Misc2Amount { get; set; }

        public decimal Misc3Amount { get; set; }

        public decimal JobToDateCostAmount { get; set; }

        public decimal LastMonthCostAmount { get; set; }

        public decimal MonthToDateCostAmount { get; set; }

        public decimal JobToDateRevenuePaymentAmount { get; set; }

        public decimal LastMonthRevenuePaymentAmount { get; set; }

        public decimal MonthToDateRevenuePaymentAmount { get; set; }

        public decimal JobToDateRevenueRetainageHeldAmount { get; set; }

        public decimal LastMonthReveueRetainageHeldAmount { get; set; }

        public decimal MonthToDateRevenueRetainageHeldAmount { get; set; }

        public decimal JobToDateWorkBilledAmount { get; set; }

        public decimal LastMonthWorkBilledAmount { get; set; }

        public decimal MonthToDateWorkBilledAmount { get; set; }

        public decimal JobToDateCostPaymentAmount { get; set; }

        public decimal LastMonthCostPaymentAmount { get; set; }

        public decimal MonthToDateCostPaymentAmount { get; set; }

        /// <summary>
        /// Categories must be manually retrieved and set
        /// </summary>
        [XmlIgnore]
        public List<Category> Categories { get; set; }

        #endregion

        #region Methods

        public int GetVersion() => Version;

        public override string ToString()
        {
            return $"{Code} | {Name}";
        }

        #endregion
    }

}
