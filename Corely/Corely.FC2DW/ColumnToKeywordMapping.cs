using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.FC2DW
{
    public class ColumnToKeywordMapping
    {
        public ColumnToKeywordMapping() { }

        /// <summary>
        /// DW Keyword field name
        /// </summary>
        public string KeywordFieldName { get; set; }

        /// <summary>
        /// FlexiCapture table name
        /// </summary>
        public string FlexiTableName { get; set; }

        /// <summary>
        /// FlexiCapture table column name
        /// </summary>
        public string FlexiTableColumn { get; set; }

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
            if (string.IsNullOrWhiteSpace(FlexiTableName))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(FlexiTableColumn))
            {
                return false;
            }
            return true;
        }
    }
}
