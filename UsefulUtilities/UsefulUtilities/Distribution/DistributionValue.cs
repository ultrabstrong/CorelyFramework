using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Distribution
{
    [Serializable]
    public class DistributionValue
    {
        #region Contstructor

        public DistributionValue() { }

        public DistributionValue(decimal _value)
        {
            Value = _value;
        }

        #endregion

        #region Properties

        public decimal Value { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Value.ToString();
        }

        #endregion
    }
}
