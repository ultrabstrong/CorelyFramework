using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Sage300HH2.Core
{
    [Serializable]
    public class SyncFailure : IVersioned
    {
        #region Properties

        public DateTime CreatedOnUtc { get; set; }

        public string EntityId { get; set; }

        public string ErrorMessage { get; set; }

        public string Id { get; set; }

        public string TypeId { get; set; }

        public int Version { get; set; }

        #endregion

        #region Methods

        public int GetVersion() => Version;

        public override string ToString()
        {
            return ErrorMessage;
        }

        #endregion
    }
}
