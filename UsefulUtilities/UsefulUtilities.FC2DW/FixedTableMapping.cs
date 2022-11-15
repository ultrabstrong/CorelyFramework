using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.FC2DW
{
    public class FixedTableMapping
    {
        /// <summary>
        /// Init with fixed column values
        /// </summary>
        public FixedTableMapping()
        {
            FixedRowColumnMapping = new List<Dictionary<string, string>>();
        }

        /// <summary>
        /// DW Table field name
        /// </summary>
        public string DWTableName { get; set; }

        /// <summary>
        /// List of rows with column / value mapping
        /// </summary>
        public List<Dictionary<string, string>> FixedRowColumnMapping { get; set; }


        /// <summary>
        /// Validate this object
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            // Validate DW table name exists
            if (string.IsNullOrWhiteSpace(DWTableName))
            {
                return false;
            }
            // Validate rows exist
            if (FixedRowColumnMapping == null || FixedRowColumnMapping.Count == 0)
            {
                return false;
            }
            // Validate that rows have mapped columns
            foreach (Dictionary<string, string> row in FixedRowColumnMapping)
            {
                if (row.Keys.Count == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
