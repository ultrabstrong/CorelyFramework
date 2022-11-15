using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Kingstone.Core
{
    [Serializable]
    public class Payment
    {
        public int? PolicyId { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public string PayorName { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }

        public string PaymentSource { get; set; }

        public string BankRoutingIdLast4 { get; set; }

        public string BankAccountIdLast4 { get; set; }

        public string CCLast4 { get; set; }

        public string CCSecurityCode { get; set; }

        public string CCExpirationDate { get; set; }

        public string ConfirmationCode { get; set; }

        public decimal? Amount { get; set; }

        public string CheckNumber { get; set; }

        public DateTime? CheckDate { get; set; }
    }
}
