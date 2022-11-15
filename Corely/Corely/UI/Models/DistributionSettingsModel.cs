using Corely.Distribution;
using Corely.UI.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Corely.UI.Models
{
    internal class DistributionSettingsModel : NotifyBase
    {
        #region Constructor

        /// <summary>
        /// Construct this object
        /// </summary>
        public DistributionSettingsModel() { }

        /// <summary>
        /// Construct this object with underlying settings
        /// </summary>
        /// <param name="distributionSettings"></param>
        public DistributionSettingsModel(DistributionSettings distributionSettings) : this()
        {
            DistributionSettings = distributionSettings;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Underlying distribution settings
        /// </summary>
        public DistributionSettings DistributionSettings
        {
            get
            {
                // Set distribution values list
                _distributionSettings.Distributions = Distributions?.DistributionValues;
                // Return distribution
                return _distributionSettings;
            }
            set
            {
                // Set distribuiton settings and values list
                _distributionSettings = value;
                if (_distributionSettings != null)
                {
                    Distributions = new DistributionValuesModel(_distributionSettings.Distributions);
                }
            }
        }
        private DistributionSettings _distributionSettings = new DistributionSettings();
        
        /// <summary>
        /// User-friendly name of distribution
        /// </summary>
        public string Name
        {
            get => DistributionSettings.Name;
            set => SetProp(DistributionSettings, m => m.Name, value, nameof(Name));
        }

        /// <summary>
        /// Type of the custom distribution
        /// </summary>
        public DistributionType DistributionType
        {
            get => DistributionSettings.DistributionType;
            set => SetProp(DistributionSettings, m => m.DistributionType, value, nameof(DistributionType));
        }

        /// <summary>
        /// Number of places to round value
        /// </summary>
        public int RoundToPlaces
        {
            get => DistributionSettings.RoundToPlaces;
            set => SetProp(DistributionSettings, m=> m.RoundToPlaces, value, nameof(RoundToPlaces));
        }

        /// <summary>
        /// Set value to one
        /// </summary>
        public bool SetValueToOne
        {
            get => DistributionSettings.SetValueToOne;
            set => SetProp(DistributionSettings, m => m.SetValueToOne, value, nameof(SetValueToOne));
        }

        /// <summary>
        /// Number of distributions for fixed dist
        /// </summary>
        public int FixedDistributionCount
        {
            get => DistributionSettings.FixedDistributionCount;
            set => SetProp(DistributionSettings, m => m.FixedDistributionCount, value, nameof(FixedDistributionCount));
        }

        /// <summary>
        /// Collection of distributions
        /// </summary>
        public DistributionValuesModel Distributions { get; set; } = new DistributionValuesModel();

        /// <summary>
        /// Minimum value allowed for distribution
        /// </summary>
        public decimal MinValue
        {
            get => DistributionSettings.MinValue;
            set
            {
                if (value >= 0.0m && value <= MaxValue)
                {
                    SetProp(DistributionSettings, m => m.MinValue, value, nameof(MinValue));
                }
            }
        }

        /// <summary>
        /// Maximum value allowed for distribution
        /// </summary>
        public decimal MaxValue
        {
            get => DistributionSettings.MaxValue;
            set
            {
                if (value >= MinValue)
                {
                    SetProp(DistributionSettings, m => m.MaxValue, value, nameof(MaxValue));
                }
            }
        }

        #endregion

    }
}
