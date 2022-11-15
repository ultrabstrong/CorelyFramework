using UsefulUtilities.Distribution;
using UsefulUtilities.UI.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UsefulUtilities.UI.Models
{
    internal class DistributionValueModel : NotifyBase
    {
        #region Constructor

        /// <summary>
        /// Base constructor
        /// </summary>
        public DistributionValueModel() { }

        /// <summary>
        /// Constructor with value
        /// </summary>
        /// <param name="value"></param>
        public DistributionValueModel(decimal value) : this()
        {
            DistributionValue.Value = value;
        }

        /// <summary>
        /// Constructor with underlying object
        /// </summary>
        /// <param name="distributionValue"></param>
        public DistributionValueModel(DistributionValue distributionValue) : this()
        {
            DistributionValue = distributionValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Underlying distribution value
        /// </summary>
        public DistributionValue DistributionValue { get; internal set; } = new DistributionValue();

        /// <summary>
        /// Decimal value
        /// </summary>
        public decimal Value
        {
            get => DistributionValue.Value;
            set
            {
                SetProp(DistributionValue, m => m.Value, value, nameof(Value));
                ValueChanged?.Invoke();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value changed event
        /// </summary>
        public event Action ValueChanged;

        /// <summary>
        /// Return value
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value.ToString();

        #endregion

    }
}
