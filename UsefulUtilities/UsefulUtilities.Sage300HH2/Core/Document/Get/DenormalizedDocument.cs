using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Sage300HH2.Core.Document
{
    [Serializable]
    public class DenormalizedDocument : IVersioned
    {
        #region Properties

        public string Id { get; set; }

        public int Version { get; set; }

        public List<DenormalizedDistribution> Distributions { get; set; }

        public LastAction LastAction { get; set; }

        #endregion

        #region Methods
        public int GetVersion() => Version;

        public override string ToString()
        {
            return Id;
        }

        #endregion
    }
}
