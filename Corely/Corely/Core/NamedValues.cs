using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Core
{
    [Serializable]
    public class NamedValues : List<NamedValue>
    {
        #region Constructors

        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public NamedValues() { }

        /// <summary>
        /// Construct from value pairs
        /// </summary>
        /// <param name="valuePairs"></param>
        public NamedValues(Dictionary<string, string> valuePairs) 
        { 
            if(valuePairs != null)
            {
                foreach(KeyValuePair<string, string> valuePair in valuePairs)
                {
                    this.Add(valuePair.Key, valuePair.Value);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// String accessor
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name]
        {
            get => this.FirstOrDefault(m => m.Name == name)?.Value;
            set
            {
                // Get value by value name if it exists
                NamedValue existingValue = this.FirstOrDefault(m => m.Name == name);
                if (existingValue != null)
                {
                    // Update existing value
                    existingValue.Value = value;
                }
                else
                {
                    // Add new value
                    Add(new NamedValue(name, value));
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create and add named value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, string value)
        {
            Add(new NamedValue(name, value));
        }

        /// <summary>
        /// Remove value for value name
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            this.RemoveAll(m => m.Name == name);
        }

        /// <summary>
        /// Sort named values
        /// </summary>
        public new void Sort()
        {
            base.Sort(Comparer<NamedValue>.Create((x, y) => x.Name.CompareTo(y.Name)));
        }

        #endregion
    }
}
