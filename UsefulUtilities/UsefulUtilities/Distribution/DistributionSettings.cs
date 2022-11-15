using UsefulUtilities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rm = UsefulUtilities.Resources.Distribution.DistributionSettings;

namespace UsefulUtilities.Distribution
{
    [Serializable]
    public class DistributionSettings
    {
        #region Constructor

        /// <summary>
        /// Construct this object
        /// </summary>
        public DistributionSettings() { }

        #endregion

        #region Properties

        /// <summary>
        /// Name of distribution
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id of distribution
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Type of distribution
        /// </summary>
        public DistributionType DistributionType { get; set; }

        /// <summary>
        /// Number of places to round decimal
        /// </summary>
        public int RoundToPlaces { get; set; }

        /// <summary>
        /// Set value to one
        /// </summary>
        public bool SetValueToOne { get; set; }

        /// <summary>
        /// Number of distributions for fixed distribution
        /// </summary>
        public int FixedDistributionCount { get; set; }

        /// <summary>
        /// Decimal disribution weights
        /// </summary>
        public List<DistributionValue> Distributions { get; set; }

        /// <summary>
        /// Minimum value allowed to distribute
        /// </summary>
        public decimal MinValue { get; set; } = 2.0M;

        /// <summary>
        /// Maximum value allowed to distribute
        /// </summary>
        public decimal MaxValue { get; set; } = 40.0M;

        #endregion

        #region Methods

        /// <summary>
        /// Check if value is within min and max range
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsValueValid(decimal value)
        {
            return MinValue <= value && value <= MaxValue;
        }

        /// <summary>
        /// Get distribution for a given value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<decimal> GetDistribtuion(decimal value)
        {
            // Verify value is in range
            List<decimal> dist = null;
            if (DistributionType == DistributionType.Fixed || SetValueToOne || (MinValue <= value && value <= MaxValue))
            {
                // Create local vars for getting distributions
                dist = new List<decimal>();
                decimal val = value;
                // Fill distribution list for distribution type
                if (DistributionType == DistributionType.Basic)
                {
                    // Fill distribution list with [value - 1] number of ones
                    while (val > 1.0M)
                    {
                        dist.Add(1.0M);
                        val -= 1.0M;
                    }
                    // Add any remaining value to last row
                    if (val != 0.0M)
                    {
                        dist.Add(ValueOrOne(val));
                    }
                }
                else if (DistributionType == DistributionType.Fixed)
                {
                    if (SetValueToOne)
                    {
                        // Fill distribution list with FixedDistributionCount number of 1.0s
                        dist = Enumerable.Repeat(1.0M, FixedDistributionCount).ToList();
                    }
                    else
                    {
                        // Get fixed value for distribution denominator
                        decimal d = Math.Round(value / FixedDistributionCount, RoundToPlaces);
                        // Fill distribution list with as many whole values as possible
                        while (val > d)
                        {
                            dist.Add(d);
                            val -= d;
                        }
                        // Add any remaining value to last row
                        if (val != 0.0M)
                        {
                            dist.Add(val);
                        }
                    }
                }
                else if (DistributionType == DistributionType.Value)
                {
                    // Fill distribution list until value runs out
                    foreach (DistributionValue d in Distributions)
                    {
                        // Deduct distribution from value
                        val -= d.Value;
                        if (val >= 0.0M)
                        {
                            // Distribution did not overuse value, so add it as is
                            dist.Add(ValueOrOne(d.Value));
                            if (val == 0.0M)
                            {
                                // Value is used up. Stop distributing.
                                break;
                            }
                        }
                        else
                        {
                            // Distribution overused value. Modify to only use availabe value and stop distributing
                            dist.Add(ValueOrOne(d.Value + val));
                            break;
                        }
                    }
                    // If distribution did not use entire value then add value as the last entry
                    if (val > 0.0M && !SetValueToOne)
                    {
                        dist[dist.Count - 1] += val;
                    }
                }
                else if (DistributionType == DistributionType.Percent)
                {
                    // Fill distribution list until value runs out
                    foreach (DistributionValue ratio in Distributions)
                    {
                        decimal d = Math.Round(value * ratio.Value / 100, RoundToPlaces);
                        // Deduct distribution from value
                        val -= d;
                        if (val >= 0.0M)
                        {
                            // Distribution did not overuse value, so add it as is
                            dist.Add(ValueOrOne(d));
                            if (val == 0.0M)
                            {
                                // Value is used up. Stop distributing.
                                break;
                            }
                        }
                        else
                        {
                            // Distribution overused value. Modify to only use availabe value and stop distributing
                            dist.Add(ValueOrOne(d + val));
                            break;
                        }
                    }
                    // If distribution did not use entire value then add value as the last entry
                    if (val > 0.0M && !SetValueToOne)
                    {
                        dist[dist.Count - 1] += val;
                    }
                }
            }
            return dist;
        }

        /// <summary>
        /// Return value or 1.0 for SetValueToOne flag
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private decimal ValueOrOne(decimal value)
        {
            return SetValueToOne ? 1.0M : value;
        }

        /// <summary>
        /// Check if properties are valid
        /// </summary>
        /// <returns></returns>
        public void IsValid(ResultBase result)
        {
            // Validate min value
            if (MinValue < 0.0M)
            {
                result.AddDataError($"{nameof(DistributionSettings)}.{nameof(MinValue)} {rm.propertyError} : {rm.mustBe} >= 0.0");
            }
            if (MinValue > MaxValue)
            {
                result.AddDataError($"{nameof(DistributionSettings)}.{nameof(MinValue)} {rm.propertyError} : {rm.mustBe} <= {nameof(DistributionSettings)}.{nameof(MinValue)}");
            }
            // Validate max value
            if (MaxValue < 0.0M)
            {
                result.AddDataError($"{nameof(DistributionSettings)}.{nameof(MaxValue)} {rm.propertyError} : {rm.mustBe} >= 0.0");
            }
            // Validate distribution values for non-basic distributions
            if (DistributionType != DistributionType.Basic && DistributionType != DistributionType.Fixed && (Distributions == null || Distributions.Count == 0))
            {
                result.AddDataError($"{nameof(DistributionSettings)}.{nameof(Distributions)} {rm.propertyError} : {rm.mustHaveOne}");
            }
            // Validate distribution values for percent distributions
            if (DistributionType == DistributionType.Percent && Distributions != null)
            {
                decimal distsum = 0.0M;
                foreach (DistributionValue dist in Distributions)
                {
                    distsum += dist.Value;
                }
                if (distsum != 100.0M)
                {
                    result.AddDataError($"{nameof(DistributionSettings)}.{nameof(Distributions)} {rm.propertyError} : {rm.mustSumTo100}");
                }
            }
            // Validate round to places
            if (DistributionType == DistributionType.Percent && RoundToPlaces < 0)
            {
                result.AddDataError($"{nameof(DistributionSettings)}.{nameof(RoundToPlaces)} {rm.propertyError} : {rm.mustBe} >= 0");
            }
        }

        #endregion
    }
}
