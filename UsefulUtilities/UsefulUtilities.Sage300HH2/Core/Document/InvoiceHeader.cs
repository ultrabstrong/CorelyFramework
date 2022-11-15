using System;

namespace UsefulUtilities.Sage300HH2.Core.Document
{

    [Serializable]
    public class InvoiceHeader
    {
        public DateTime? AccountingDate { get; set; }
        public decimal? Amount { get; set; }
        public string Code { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime? DiscountDate { get; set; }
        public string InvoiceCode1 { get; set; }
        public string InvoiceCode2 { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? MiscellaneousAmount { get; set; }
        public string Name { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string SummaryPayeeAddress1 { get; set; }
        public string SummaryPayeeAddress2 { get; set; }
        public string SummaryPayeeCity { get; set; }
        public string SummaryPayeeName { get; set; }
        public string SummaryPayeeState { get; set; }
        public string SummaryPayeeZip { get; set; }
        public decimal? TaxAmount { get; set; }
        public string VendorId { get; set; }
        public string InvoiceCity { get; set; }
        public string InvoiceState { get; set; }
        public string ContractId { get; set; }
        public string CustomerId { get; set; }
        public string Reference { get; set; }
        public string CommitmentId { get; set; }
        public string TaxGroupId { get; set; }
    }
}
