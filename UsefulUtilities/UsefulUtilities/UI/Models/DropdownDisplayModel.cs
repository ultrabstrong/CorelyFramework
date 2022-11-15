using UsefulUtilities.Core;
using UsefulUtilities.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.UI.Models
{
    public class DropdownDisplayModel : NotifyBase
    {
        #region Constructor

        /// <summary>
        /// Construct with display name
        /// </summary>
        public DropdownDisplayModel(string display) 
        {
            DisplayName = display;
        }

        /// <summary>
        /// Construct with display name and additional values
        /// </summary>
        /// <param name="display"></param>
        /// <param name="values"></param>
        public DropdownDisplayModel(string display, NamedValues values) 
        {
            DisplayName = display;
            AdditionalData = values;
        }

        /// <summary>
        /// Easy construct with display name and additional values
        /// </summary>
        /// <param name="display"></param>
        /// <param name="values"></param>
        public DropdownDisplayModel(string display, Dictionary<string, string> values)
        {
            DisplayName = display;
            AdditionalData = new NamedValues(values);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Display text for item
        /// </summary>
        public string DisplayName
        {
            get => _displayName;
            set => SetField(ref _displayName, value, nameof(DisplayName));
        }
        private string _displayName = "";

        /// <summary>
        /// Additional data for item
        /// </summary>
        public NamedValues AdditionalData { get; set; }

        #endregion

    }
}
