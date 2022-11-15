using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Connections
{
    public class PagedResult<T> where T : class
    {
        #region Constructor

        /// <summary>
        /// Construct this object
        /// </summary>
        public PagedResult(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Items result
        /// </summary>
        public List<T> Items { get; private set; } = new List<T>();

        /// <summary>
        /// Return calculated page
        /// </summary>
        public int PageNum => Skip / Take;

        /// <summary>
        /// Number of records to skip
        /// </summary>
        public int Skip { get; private set; }

        /// <summary>
        /// Number of records to take
        /// </summary>
        public int Take { get; private set; }

        /// <summary>
        /// Are there more results
        /// </summary>
        public bool HasMore { get; private set; } = true;

        #endregion

        #region Methods

        /// <summary>
        /// Update page properties
        /// </summary>
        /// <param name="itemscount"></param>
        private void UpdatePage(int itemscount)
        {
            Skip += itemscount;
            HasMore = itemscount == Take;
        }

        /// <summary>
        /// Set items 
        /// </summary>
        /// <param name="items"></param>
        public void SetItems(List<T> items)
        {
            UpdatePage(items?.Count ?? 0);
            Items = items;
        }

        /// <summary>
        /// Add items 
        /// </summary>
        /// <param name="items"></param>
        public void AddItems(List<T> items)
        {
            UpdatePage(items?.Count ?? 0);
            Items.AddRange(items);
        }

        /// <summary>
        /// Get next chunk
        /// </summary>
        public PagedResult<T> GetNextChunk()
        {
            if(HasMore == false || _onGetNextChunk == null)
            {
                return null;
            }
            return _onGetNextChunk?.Invoke(this);
        }

        /// <summary>
        /// Delegate and event for getting next chunk
        /// </summary>
        /// <param name="currentChunk"></param>
        /// <returns></returns>
        public delegate PagedResult<T> GetNextChunkDelegate(PagedResult<T> currentChunk);
        public event GetNextChunkDelegate OnGetNextChunk
        {
            add
            {
                // Protect against duplicate registration
                _onGetNextChunk -= value;
                _onGetNextChunk += value;
            }
            remove
            {
                _onGetNextChunk -= value;
            }
        }
        private event GetNextChunkDelegate _onGetNextChunk;

        #endregion
    }
}
