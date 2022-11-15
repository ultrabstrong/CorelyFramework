using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.FlexiCapture
{
    [Serializable]
    public class CopyFields : List<CopyField>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CopyFields() { }

        /// <summary>
        /// Easy constructor
        /// </summary>
        /// <param name="copyField"></param>
        public CopyFields(string copyField) : this(new CopyField(copyField)) { }

        /// <summary>
        /// Easy constructor
        /// </summary>
        /// <param name="copyFrom"></param>
        /// <param name="copyTo"></param>
        public CopyFields(string copyFrom, string copyTo) : this(new CopyField(copyFrom, copyTo)) { }

        /// <summary>
        /// Easy constructor for multiple fields
        /// </summary>
        /// <param name="fields"></param>
        public CopyFields(params CopyField[] fields)
        {
            if (fields != null)
            {
                this.AddRange(fields.ToList());
            }
        }

        #endregion
    }
}
