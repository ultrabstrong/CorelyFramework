using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.FlexiCapture
{
    [Serializable]
    public class AdditionalDistributeFields : List<AdditionalDistributeField>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public AdditionalDistributeFields() { }

        /// <summary>
        /// Easy constructor
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="useInRowCalculation"></param>
        /// <param name="rowCalcOperation"></param>
        public AdditionalDistributeFields(string fieldName, bool useInRowCalculation, RowCalcOperation rowCalcOperation) : this(new AdditionalDistributeField(fieldName, useInRowCalculation, rowCalcOperation)) { }

        /// <summary>
        /// Easy constructor for multiple fields
        /// </summary>
        /// <param name="fields"></param>
        public AdditionalDistributeFields(params AdditionalDistributeField[] fields)
        {
            if(fields != null)
            {
                this.AddRange(fields.ToList());
            }
        }

        #endregion
    }
}
