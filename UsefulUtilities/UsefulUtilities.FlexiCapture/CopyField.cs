using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.FlexiCapture
{
    [Serializable]
    public class CopyField
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CopyField() { }

        /// <summary>
        ///  Easy constructor
        /// </summary>
        /// <param name="copyField"></param>
        public CopyField(string copyField)
        {
            CopyFrom = copyField;
            CopyTo = copyField;
        }

        /// <summary>
        ///  Easy constructor
        /// </summary>
        /// <param name="copyFrom"></param>
        /// <param name="copyTo"></param>
        public CopyField(string copyFrom, string copyTo)
        {
            CopyFrom = copyFrom;
            CopyTo = copyTo;
        }

        /// <summary>
        ///  Easy constructor
        /// </summary>
        /// <param name="copyFrom"></param>
        /// <param name="copyTo"></param>
        public CopyField(string copyFrom, string copyTo, string copyValue)
        {
            CopyFrom = copyFrom;
            CopyTo = copyTo;
            CopyValue = copyValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Field name to copy from
        /// </summary>
        public string CopyFrom { get; set; }

        /// <summary>
        /// Field name to copy to
        /// </summary>
        public string CopyTo { get; set; }

        /// <summary>
        /// Copy value
        /// </summary>
        public string CopyValue { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}
