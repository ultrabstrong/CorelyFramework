using Corely.Data.Serialization;
using Corely.UI.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using rm = Corely.Resources.UI.DistributionSettingsUC;

namespace Corely.UI
{
    public partial class DistributionSettingsUC : UserControl
    {
        #region Constructor / Initialization

        /// <summary>
        /// Construct this user control
        /// </summary>
        public DistributionSettingsUC()
        {
            Settings = new DistributionSettingsModel
            {
                Name = rm.runOnceSettings
            };
            InitializeComponent();
        }

        /// <summary>
        /// Construct this user control with settings
        /// </summary>
        public DistributionSettingsUC(string settingsxml)
        {
            Settings = XmlSerializer.DeSerialize<DistributionSettingsModel>(settingsxml);
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Settings dependency property
        /// </summary>
        internal static readonly DependencyProperty SettingsProperty =
            DependencyProperty.Register(nameof(Settings), typeof(DistributionSettingsModel), typeof(DistributionSettingsUC),
                new FrameworkPropertyMetadata(default(DistributionSettingsModel), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Distribution settings
        /// </summary>
        internal DistributionSettingsModel Settings
        {
            get => (DistributionSettingsModel)GetValue(SettingsProperty);
            set => SetValue(SettingsProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Remove distribution row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeRowButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings.Distributions.Remove((DistributionValueModel)((Image)sender).DataContext);
        }

        /// <summary>
        /// Remove distribution row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addRowButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings.Distributions.Add(new DistributionValueModel(0.0M));
        }

        #endregion

    }
}
