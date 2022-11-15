using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.FC2DW
{
    public class FixedKeywordMapping
    {
        public FixedKeywordMapping() { }

        /// <summary>
        /// DW Keyword field name
        /// </summary>
        public string KeywordFieldName { get; set; }

        /// <summary>
        /// Fixed list of keywords
        /// </summary>
        public List<string> Keywords { get; set; }

        /// <summary>
        /// Validate this object
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(KeywordFieldName))
            {
                return false;
            }
            if (Keywords == null || Keywords.Count == 0)
            {
                return false;
            }
            return true;
        }
    }
}
