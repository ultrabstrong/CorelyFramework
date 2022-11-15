using UsefulUtilities.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using rm = UsefulUtilities.Resources.UI.DropdownDisplay;

namespace UsefulUtilities.UI
{
    /// <summary>
    /// Interaction logic for DropDownWindow.xaml
    /// </summary>
    public partial class DropdownDisplay : Window, INotifyPropertyChanged
    {
        #region Constructor / Initialization

        /// <summary>
        /// Construct this window
        /// </summary>
        public DropdownDisplay()
        {
            InitializeComponent();
            DataContext = this;
            Title = rm.dropdownSelection;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Display text
        /// </summary>
        public string DisplayText {
            get => _displayText;
            set => SetField(ref _displayText, value, nameof(DisplayText));
        }
        private string _displayText = "";

        /// <summary>
        /// Dropdown items to display
        /// </summary>
        public List<DropdownDisplayModel> DropdownItems
        {
            get => _dropdownItems;
            set
            {
                if(value != null)
                {
                    SetField(ref _dropdownItems, value, nameof(DropdownItems));
                }
            }
        }
        private List<DropdownDisplayModel> _dropdownItems = new List<DropdownDisplayModel>();

        /// <summary>
        /// Result from message box
        /// </summary>
        public Core.MessageBoxResult Result { get; set; } = Core.MessageBoxResult.None;

        /// <summary>
        /// Get selected dropdown item
        /// </summary>
        public DropdownDisplayModel SelectedItem
        {
            get
            {
                if (dropdownItems.SelectedItem == null) { return null; }
                return (DropdownDisplayModel)dropdownItems.SelectedItem;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set ok result and close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Result = Core.MessageBoxResult.Ok;
            Close();
        }

        /// <summary>
        /// Set cancel result and close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = Core.MessageBoxResult.Cancel;
            Close();
        }

        #endregion

        #region Notify Changed Event

        /// <summary>
        /// Change a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            Notify(propertyName);
            return true;
        }

        /// <summary>
        /// Notify property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
