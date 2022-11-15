using System;
namespace UsefulUtilities.Sage300HH2.Core
{
    [Serializable]
    public class Category : IVersioned
    {
        #region Properties

        public string Id { get; set; }

        public int Version { get; set; }

        public decimal ApprovedCommitmentChanges { get; set; }

        public decimal ApprovedEstimateChanges { get; set; }

        public decimal ApprovedEstimateUnitChanges { get; set; }

        public string Code { get; set; }

        public decimal CommitmentInvoiced { get; set; }

        public string CostCodeId { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public string Description { get; set; }

        public decimal Estimate { get; set; }

        public string EstimateUnitOfMeasure { get; set; }

        public decimal EstimateUnits { get; set; }

        public bool HasExternalId { get; set; }

        public bool IsActive { get; set; }

        public bool IsArchived { get; set; }

        public string JobId { get; set; }

        public decimal JobToDateCost { get; set; }

        public decimal JobToDateDollarsPaid { get; set; }

        public decimal JobToDateUnits { get; set; }

        public decimal MonthToDateCost { get; set; }

        public decimal MonthToDateDollarsPaid { get; set; }

        public decimal MonthToDateUnits { get; set; }

        public string Name { get; set; }

        public decimal OriginalCommitment { get; set; }

        public decimal OriginalEstimate { get; set; }

        public decimal OriginalEstimateUnits { get; set; }

        public decimal PercentComplete { get; set; }

        public decimal RevisedCommitment { get; set; }

        public string StandardCategoryId { get; set; }

        public string StandardCategoryAccumulationName { get; set; }

        public string StandardCategoryDescription { get; set; }

        public string StandardCategoryName { get; set; }

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
