using Corely.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using srrm = Corely.Resources.UI.DistributionSettingsUC;
using rm = Corely.Resources.UI.DistributionSettingsListUC;
using Corely.Core;
using Corely.Distribution;

namespace Corely.UI
{
    public partial class DistributionSettingsListUC : UserControl
    {
        #region Constructor / Initialization

        /// <summary>
        /// Construct this user control
        /// </summary>
        public DistributionSettingsListUC()
        {
            Settings = new DistributionSettingsListModel();
            InitializeComponent();
        }

        /// <summary>
        /// Construct this user control with settings
        /// </summary>
        public DistributionSettingsListUC(List<DistributionSettings> settings)
        {
            Settings = new DistributionSettingsListModel(settings);
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Settings dependency property
        /// </summary>
        internal static readonly DependencyProperty SettingsProperty =
            DependencyProperty.Register(nameof(Settings), typeof(DistributionSettingsListModel), typeof(DistributionSettingsListUC),
                new FrameworkPropertyMetadata(default(DistributionSettingsListModel), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Distribution settings
        /// </summary>
        internal DistributionSettingsListModel Settings
        {
            get => (DistributionSettingsListModel)GetValue(SettingsProperty);
            set => SetValue(SettingsProperty, value);
        }

        /// <summary>
        /// Hide save dependency property
        /// </summary>
        internal static readonly DependencyProperty HideSaveProperty =
            DependencyProperty.Register(nameof(HideSave), typeof(bool), typeof(DistributionSettingsListUC),
                new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Show save flag
        /// </summary>
        internal bool HideSave {

            get => (bool)GetValue(HideSaveProperty);
            set => SetValue(HideSaveProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// OnSave event to trigger when save button clicked
        /// </summary>
        public event Action OnSave;
        
        /// <summary>
        /// Remove settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSettingsButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings.Add(new DistributionSettingsModel() { Name = srrm.runOnceSettings });
            settingsDropdown.SelectedItem = Settings[Settings.Count - 1];
        }

        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveSettingsButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnSave?.Invoke();
        }

        /// <summary>
        /// Remove settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSettingsButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DistributionSettingsModel settings = (DistributionSettingsModel)settingsDropdown.SelectedItem;
            Settings.Remove(settings);
        }

        /// <summary>
        /// Copy XML settings to clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopySettingsButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string settingsXml = Corely.Data.Serialization.XmlSerializer.Serialize(Settings.DistributionSettings);
            Clipboard.SetText(settingsXml);
            Corely.Core.Message.Show(rm.settingsCopied);
        }

        /// <summary>
        /// Update settings model
        /// </summary>
        /// <param name="settings"></param>
        public void SetSettings(List<DistributionSettings> settings)
        {
            Settings = new DistributionSettingsListModel(settings);
        }

        /// <summary>
        /// Get selected settings
        /// </summary>
        /// <returns></returns>
        public DistributionSettings GetSelectedSettings()
        {
            if (settingsDropdown.SelectedItem == null) { return null; }
            return ((DistributionSettingsModel)settingsDropdown.SelectedItem).DistributionSettings;
        }

        /// <summary>
        /// Get settings XML
        /// </summary>
        /// <returns></returns>
        public List<DistributionSettings> GetSettings()
        {
            return Settings.DistributionSettings;
        }

        #endregion

    }
}
