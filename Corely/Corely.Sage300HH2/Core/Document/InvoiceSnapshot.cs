using System;
using System.Collections.Generic;

namespace Corely.Sage300HH2.Core.Document
{

    [Serializable]
    public class InvoiceSnapshot
    {
        public List<InvoiceDistribution> Distributions { get; set; }
        public InvoiceHeader Header { get; set; }
        public bool ShouldSuspendInAP { get; set; }
        public string SuspendInAPReason { get; set; }
    }
}
