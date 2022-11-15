using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Sage300HH2.Core
{
    [Serializable]
    public partial class Payment : IVersioned
    {
        #region Properties

        public string AccountReference { get; set; }
                
        public decimal? Amount { get; set; }
                
        public string Code { get; set; }

        public bool? HasExternalId { get; set; }
                
        public string Id { get; set; }
                
        public bool? IsArchived { get; set; }

        public DateTime? PaymentDate { get; set; }
                
        public string Reference1 { get; set; }

        public string Status { get; set; }
                
        public PaymentType Type { get; set; }
                
        public string VendorId { get; set; }
                
        public int Version { get; set; }

        #endregion

        #region Methods

        public int GetVersion() => Version;

        public override string ToString()
        {
            return $"{Code} | {Amount}";
        }

        #endregion
    }


}
