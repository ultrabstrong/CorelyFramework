using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using rm = Corely.Resources.UI.MessageBox;

namespace Corely.UI
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageDisplay : Window, INotifyPropertyChanged
    {

        #region Constructor / Initialization

        /// <summary>
        /// Construct this window
        /// </summary>
        public MessageDisplay()
        {
            InitializeComponent();
            DataContext = this;
            Title = rm.message;
            ShowOk = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Show cancel button
        /// </summary>
        public bool ShowCancel
        {
            get { return _showCancel; }
            set
            {
                SetField(ref _showCancel, value, nameof(ShowCancel));
                this.Refresh();
            }
        }
        private bool _showCancel;

        /// <summary>
        /// Show ok button
        /// </summary>
        public bool ShowOk
        {
            get { return _showOk; }
            set
            {
                SetField(ref _showOk, value, nameof(ShowOk));
                this.Refresh();
            }
        }
        private bool _showOk;

        /// <summary>
        /// Text to display
        /// </summary>
        public string DisplayText
        {
            get { return _displayText; }
            set
            {
                SetField(ref _displayText, value, nameof(DisplayText));
                this.Refresh();
            }
        }
        private string _displayText;

        /// <summary>
        /// Result from message box
        /// </summary>
        public Core.MessageBoxResult Result { get; set; } = Core.MessageBoxResult.None;

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
