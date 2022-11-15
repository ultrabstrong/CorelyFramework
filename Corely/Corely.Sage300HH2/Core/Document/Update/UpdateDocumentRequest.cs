using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Sage300HH2.Core.Document
{
    [Serializable]
    public class UpdateDocumentRequest
    {
        public int ActionLevel { get; set; }
        public string UpdatedOn { get; set; }
        public InvoiceSnapshot Snapshot { get; set; }
        public string ActivityId { get; set; }
    }
}
