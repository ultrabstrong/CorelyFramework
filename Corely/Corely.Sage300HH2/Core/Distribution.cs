using System;

namespace Corely.Sage300HH2.Core
{
    [Serializable]
    public class Distribution : IVersioned
    {
        #region Properties

        public string AccountsPayableAccountId { get; set; }

        public decimal Amount { get; set; }

        public string Authorization { get; set; }

        public string BillingStandardItemId { get; set; }

        public string CategoryId { get; set; }

        public string Code { get; set; }

        public string CommitmentId { get; set; }

        public string CommitmentItemId { get; set; }

        public string CostCodeId { get; set; }

        public string Description { get; set; }

        public decimal DiscountOfferedAmount { get; set; }

        public string Draw { get; set; }

        public string EquipmentCostCodeId { get; set; }

        public string EquipmentId { get; set; }

        public decimal EquipmentMeterOdometer { get; set; }

        public string ExpenseAccountId { get; set; }

        public bool HasExternalId { get; set; }

        public string Id { get; set; }

        public string InvoiceId { get; set; }

        public bool Is1099Exempt { get; set; }

        public bool IsActive { get; set; }

        public bool IsArchived { get; set; }

        public string JobId { get; set; }

        public string JointPayee { get; set; }

        public decimal MiscDeduction2Percent { get; set; }

        public decimal MiscDeductionAmount { get; set; }

        public string MiscEntry1 { get; set; }

        public decimal MiscEntry1Units { get; set; }

        public string MiscEntry2 { get; set; }

        public decimal MiscEntry2Units { get; set; }

        public decimal RetainageAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public string TaxGroupId { get; set; }

        public decimal TaxLiabilityAmount { get; set; }

        public decimal UnitCost { get; set; }

        public decimal Units { get; set; }

        public int Version { get; set; }

        #endregion

        #region Methods

        public int GetVersion() => Version;

        public override string ToString() => Code;

        #endregion
    }
}
