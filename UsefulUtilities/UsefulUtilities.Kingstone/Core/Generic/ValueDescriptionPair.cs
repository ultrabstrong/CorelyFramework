using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Kingstone.Core
{

    [Serializable]
    public class ValueDescriptionPair
    {
        public string Value { get; set; }
        public string CodeName { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Description}{CodeName}: {Value}";
        }
    }
}
