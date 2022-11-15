using System;
namespace UsefulUtilities.Sage300HH2.Core
{
    [Serializable]
    public class CostCode : IVersioned
    {
        #region Properties

        public string Code { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public decimal Estimate { get; set; }

        public decimal EstimateUnits { get; set; }

        public bool HasExternalId { get; set; }

        public string Id { get; set; }

        public bool IsActive { get; set; }

        public bool IsArchived { get; set; }

        public bool IsGroupCode { get; set; }

        public string JobId { get; set; }

        public decimal LaborEstimateUnits { get; set; }

        public decimal Misc1Amount { get; set; }

        public decimal Misc2Amount { get; set; }

        public decimal Misc3Amount { get; set; }

        public decimal Misc4Amount { get; set; }

        public decimal Misc5Amount { get; set; }

        public decimal Misc6Amount { get; set; }

        public string Name { get; set; }

        public decimal OriginalProductionUnitsEstimate { get; set; }

        public string ParentId { get; set; }

        public decimal PercentComplete { get; set; }

        public decimal ProductionEstimateUnits { get; set; }

        public string ProductionUnitOfMeasure { get; set; }

        public decimal ProductionUnitsInPlace { get; set; }

        public string StandardCostCodeDescription { get; set; }

        public string StandardCostCodeId { get; set; }

        public string StandardCostCodeName { get; set; }

        public int Version { get; set; }

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
