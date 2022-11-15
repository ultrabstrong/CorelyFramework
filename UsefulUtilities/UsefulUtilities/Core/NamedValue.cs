using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Core
{
    [Serializable]
    public class NamedValue
    {
        #region Constructors

        /// <summary>
        /// Default constructor for serialziation
        /// </summary>
        public NamedValue() { }

        /// <summary>
        /// Constructor with name value pair
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public NamedValue(string name, string value)
        {
            Name = name;
            Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of value
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Data of value
        /// </summary>
        public string Value { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Return this object's properties
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} : {Value}";
        }

        #endregion
    }
}
