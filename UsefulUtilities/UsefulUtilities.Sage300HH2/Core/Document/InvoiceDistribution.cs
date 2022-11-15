using System;

namespace UsefulUtilities.Sage300HH2.Core.Document
{
    [Serializable]
    public class InvoiceDistribution
    {
        public string AccountsPayableAccountId { get; set; }
        public decimal? Amount { get; set; }
        public string Authorization { get; set; }
        public string CategoryId { get; set; }
        public string CommitmentId { get; set; }
        public string CommitmentItemId { get; set; }
        public string CostCodeId { get; set; }
        public string Description { get; set; }
        public decimal? DiscountOffered { get; set; }
        public string Draw { get; set; }
        public string EquipmentItemId { get; set; }
        public string ExpenseAccountId { get; set; }
        public string Id { get; set; }
        public bool Is1099Exempt { get; set; }
        public string JobId { get; set; }
        public string JointPayee { get; set; }
        public decimal? MiscDeduction { get; set; }
        public decimal? MiscDeduction2Percentage { get; set; }
        public string MiscEntry1 { get; set; }
        public decimal? MiscEntry1Units { get; set; }
        public string MiscEntry2 { get; set; }
        public decimal? MiscEntry2Units { get; set; }
        public string PMChargebackDescription { get; set; }
        public DateTime? PMChargeDate { get; set; }
        public string PMChargeType { get; set; }
        public string PMItemId { get; set; }
        public string PMLeaseId { get; set; }
        public int? PMLeaseRevisionNumber { get; set; }
        public decimal? PMMarkupAmount { get; set; }
        public string PMMarkupChargeType { get; set; }
        public decimal? PMMarkupPercentage { get; set; }
        public string PMPropertyId { get; set; }
        public string PMTenantId { get; set; }
        public string PMUnitId { get; set; }
        public decimal? Retainage { get; set; }
        public bool ShouldDelete { get; set; }
        public string StandardItemId { get; set; }
        public decimal? TaxAmount { get; set; }
        public string TaxGroupId { get; set; }
        public decimal? TaxLiability { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? Units { get; set; }
        public string EquipmentCostCodeId { get; set; }
        public decimal? MeterOdometer { get; set; }
        public string StandardCategoryId { get; set; }
        public string StandardCostCodeId { get; set; }
        public string Code { get; set; }
        public string ContractItemId { get; set; }
        public string TaxRateId { get; set; }
        public string TaxStatusId { get; set; }
        public string WorkOrderId { get; set; }
        public decimal? PreTaxAmount { get; set; }
        public decimal? GstTaxAmount { get; set; }
        public decimal? HstTaxAmount { get; set; }
        public decimal? PstTaxAmount { get; set; }
        public decimal? QstTaxAmount { get; set; }
        public string DepartmentId { get; set; }
        public string LocationId { get; set; }
        public bool ShouldApplyDefaults { get; set; }
    }
}
