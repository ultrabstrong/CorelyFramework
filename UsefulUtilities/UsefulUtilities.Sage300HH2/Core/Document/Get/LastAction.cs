using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Sage300HH2.Core.Document
{
    [Serializable]
    public class LastAction
    {
        public DateTime ActionDateTimeAgnostic { get; set; }

        public string ErrorMessage { get; set; }

        public int Level { get; set; }

        public string Type { get; set; }
    }
}
