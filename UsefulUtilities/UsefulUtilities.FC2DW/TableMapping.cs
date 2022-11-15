using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.FC2DW
{
    public class TableMapping
    {
        /// <summary>
        /// Init with mapped columns dictionary
        /// </summary>
        public TableMapping() 
        {
            MappedColumns = new Dictionary<string, string>();
        }

        /// <summary>
        /// FC Table field name
        /// </summary>
        public string FlexiTableName { get; set; }

        /// <summary>
        /// DW Table field name
        /// </summary>
        public string DWTableName { get; set; }

        /// <summary>
        /// Mapped table columns
        /// </summary>
        public Dictionary<string, string> MappedColumns { get; set; }

        /// <summary>
        /// Validate this object
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            // Validate Flexi Table Name exists
            if (string.IsNullOrWhiteSpace(FlexiTableName))
            {
                return false;
            }
            // Validate DW Table Name exists
            if (string.IsNullOrWhiteSpace(DWTableName))
            {
                return false;
            }
            // Validate column mapping exists
            if(MappedColumns == null || MappedColumns.Keys.Count == 0)
            {
                return false;
            }
            return true;
        }
    }
}
