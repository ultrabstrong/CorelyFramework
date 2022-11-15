using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Kingstone.Core
{

    [Serializable]
    public class PolicyItem
    {
        public int? Id { get; set; }
        public string Type { get; set; }
        public int? DisplayOrder { get; set; }
        public List<ValueDescriptionPair> Data { get; set; }
        public override string ToString()
        {
            return $"{Type} {Id}";
        }
    }
}
