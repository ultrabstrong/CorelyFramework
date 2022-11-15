using Corely.Core;
using Corely.Distribution;
using System;
using System.Collections.Generic;
using System.Windows;
using rm = Corely.Resources.UI.Distributions;

namespace Corely.UI
{
    public partial class Distributions : System.Windows.Window
    {

        #region Contsructor / Initialization

        /// <summary>
        /// Construct this window
        /// </summary>
        public Distributions(Func<List<DistributionSettings>> loadSettings, Action<List<DistributionSettings>> saveSettings)
        {
            try
            {
                LoadSettings += loadSettings;
                SaveSettings += saveSettings;
                InitializeComponent();
                if (saveSettings == null)
                {
                    distlist.HideSave = true;
                }
            }
            catch (Exception ex)
            {
                Corely.Core.Message.Show(rm.failLoad + Environment.NewLine + ex.ToString());
            }
        }

        /// <summary>
        /// Finish setting up window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set up distribution
                distlist.OnSave += Dist_OnSave; ;
                List<DistributionSettings> distsettings = LoadSettings?.Invoke();
                distlist.SetSettings(distsettings);
            }
            catch (Exception ex)
            {
                Corely.Core.Message.Show(rm.failLoad + Environment.NewLine + ex.ToString());
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Cancelled flag
        /// </summary>
        public bool Cancelled { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// Event for loading settings
        /// </summary>
        private event Func<List<DistributionSettings>> LoadSettings;

        /// <summary>
        /// Event for saving settings
        /// </summary>
        private event Action<List<DistributionSettings>> SaveSettings;

        /// <summary>
        /// Save settings when distribution save button clicked
        /// </summary>
        private void Dist_OnSave()
        {
            try
            {
                SaveSettings?.Invoke(distlist.GetSettings());
                Corely.Core.Message.Show(rm.distSettingsSaved);
            }
            catch (Exception ex)
            {
                Corely.Core.Message.Show(rm.failSave + Environment.NewLine + ex.ToString());
            }
        }

        /// <summary>
        /// Close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Cancel and close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Cancelled = true;
            Close();
        }

        /// <summary>
        /// Return selected settings
        /// </summary>
        /// <returns></returns>
        public DistributionSettings GetSelectedSettings()
        {
            return distlist.GetSelectedSettings();
        }

        #endregion

    }
}
