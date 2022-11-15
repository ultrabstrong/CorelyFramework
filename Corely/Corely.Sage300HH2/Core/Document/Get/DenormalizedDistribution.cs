using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Sage300HH2.Core.Document
{
    [Serializable]
    public class DenormalizedDistribution
    {
        #region Properties

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public int DistributionSequence { get; set; }

        public string Id { get; set; }

        public bool Is1099Exempt { get; set; }

        public bool IsArchived { get; set; }

        #endregion

        #region Properties

        public override string ToString()
        {
            return $"{Amount} | {Id}";
        }

        #endregion

    }
}
