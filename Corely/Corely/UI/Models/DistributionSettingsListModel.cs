using Corely.Distribution;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Corely.UI.Models
{
    internal class DistributionSettingsListModel : ObservableCollection<DistributionSettingsModel>
    {
        #region Constructors

        /// <summary>
        /// Base contsturctor
        /// </summary>
        public DistributionSettingsListModel() : base() { }

        /// <summary>
        /// Constructor with collection
        /// </summary>
        /// <param name="collection"></param>
        public DistributionSettingsListModel(IEnumerable<DistributionSettingsModel> collection) : base(collection) { }

        /// <summary>
        /// Constructor with list
        /// </summary>
        /// <param name="list"></param>
        public DistributionSettingsListModel(List<DistributionSettingsModel> list) : base(list) { }

        /// <summary>
        /// Constructor with base settings list
        /// </summary>
        /// <param name="list"></param>
        public DistributionSettingsListModel(List<DistributionSettings> list) : base(list.Select(m => new DistributionSettingsModel(m))) { }

        #endregion

        #region Properties

        /// <summary>
        /// Distribution settings list
        /// </summary>
        public List<DistributionSettings> DistributionSettings
        {
            get
            {
                return this.Select(m => m.DistributionSettings).ToList();
            }
        }

        #endregion
    }
}
