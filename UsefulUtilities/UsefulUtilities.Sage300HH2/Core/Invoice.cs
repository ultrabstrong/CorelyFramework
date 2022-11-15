using UsefulUtilities.Connections;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace UsefulUtilities.Sage300HH2.Core
{
    [Serializable]
    public class Invoice : IVersioned
    {
        #region Properties
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string Code { get; set; }
        public decimal DeductionAppliedAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool HasExternalId { get; set; }
        public string ContractId { get; set; }
        public string CustomerId { get; set; }
        public string Description { get; set; }
        public string Draw { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime AccountingDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPending { get; set; }
        public bool IsSuspended { get; set; }
        public decimal RetainageAmount { get; set; }
        public decimal RetainagePaidAmount { get; set; }
        public decimal RetainageBilledAmount { get; set; }
        public decimal RetainageHeldAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TaxPaidAmount { get; set; }
        public string VendorId { get; set; }
        public InvoiceStatus Status { get; set; }
        public string Reference { get; set; }
        public int Version { get; set; }

        /// <summary>
        /// Core.Distributions must be manually retrieved and set
        /// </summary>
        [XmlIgnore]
        public List<Core.Distribution> Distribution { get; set; }

        #endregion

        #region Methods

        public int GetVersion() => Version;

        public override string ToString() => Code;

        #endregion
    }

}
