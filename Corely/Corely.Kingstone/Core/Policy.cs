using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Kingstone.Core
{
    [Serializable]
    public class Policy
    {
        public string Source { get; set; }
        public int? RenewalTerm { get; set; }
        public int? SourceId { get; set; }
        public int? PolicyId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string ProductCode { get; set; }
        public int? CompanyId { get; set; }
        public int? AgencyId { get; set; }
        public int? ProducerId { get; set; }
        public decimal? PolicyTotal { get; set; }
        public decimal? PremiumTotal { get; set; }
        public decimal? NonPremiumTotal { get; set; }
        public decimal? DirectPremiumTotal { get; set; }
        public decimal? DirectNonPremiumTotal { get; set; }
        public ValueDescriptionPair Status { get; set; }
        public ValueDescriptionPair Type { get; set; }
        public List<ValueDescriptionPair> Data { get; set; }
        public List<PolicyItem> Items { get; set; }
        public int? QuoteId { get; set; }
        public override string ToString()
        {
            return $"{PolicyId}";
        }
    }
}
