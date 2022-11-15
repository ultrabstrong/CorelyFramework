using UsefulUtilities.Connections;
using System.Collections.Generic;
using System.Linq;

namespace UsefulUtilities.Sage300HH2.Core
{
    public class HH2PagedResult<T> where T : IVersioned
    {

        #region Contstructor

        /// <summary>
        /// Construct this object
        /// </summary>
        internal HH2PagedResult() { }

        #endregion

        #region Properties

        /// <summary>
        /// Items result
        /// </summary>
        public List<T> Items { get; internal set; } = new List<T>();

        /// <summary>
        /// Max version number in items list
        /// </summary>
        public int Version { get; internal set; } = 0;

        /// <summary>
        /// Are there more results
        /// </summary>
        public bool HasMore { get; internal set; } = true;

        #endregion

        #region Methods

        /// <summary>
        /// Set items 
        /// </summary>
        /// <param name="items"></param>
        public void SetItems(List<T> items)
        {
            if (items == null || items.Count == 0)
            {
                HasMore = false;
                return;
            }
            Items = items;
            Version = Items.Max(m => m.GetVersion());
            HasMore = items.Count == 1000;
        }

        /// <summary>
        /// Add items 
        /// </summary>
        /// <param name="items"></param>
        public void AddItems(List<T> items)
        {
            if(items == null || items.Count == 0)
            {
                HasMore = false;
                return;
            }
            Items.AddRange(items);
            Version = Items.Max(m => m.GetVersion());
            HasMore = items.Count == 1000;
        }

        #endregion

    }
}
