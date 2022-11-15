using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.FlexiCapture
{
    [Serializable]
    public class AdditionalDistributeField
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public AdditionalDistributeField() { }

        /// <summary>
        /// Easy constructor
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="useInRowCalculation"></param>
        public AdditionalDistributeField(string fieldName, bool useInRowCalculation, RowCalcOperation rowCalcOperation)
        {
            FieldName = fieldName;
            UseInRowCalculation = useInRowCalculation;
            RowCalcOperation = rowCalcOperation;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Use field in row calculation
        /// </summary>
        public bool UseInRowCalculation { get; set; }

        /// <summary>
        /// Row calculation operation
        /// </summary>
        public RowCalcOperation RowCalcOperation { get; set; }

        /// <summary>
        /// Distributions
        /// </summary>
        public List<decimal> Distributions { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get row caluclation distribution at index
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public decimal GetCalcDist(int i)
        {
            if(i < Distributions?.Count && UseInRowCalculation) 
            { 
                return RowCalcOperation == RowCalcOperation.Add ? Distributions[i] : -1.0m * Distributions[i]; 
            }
            else { return 0.0m; }
        }

        /// <summary>
        /// Get row caluclation distribution at index
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public decimal GetDist(int i)
        {
            if (i < Distributions?.Count) { return Distributions[i]; }
            else { return 0.0m; }
        }

        #endregion
    }
}
