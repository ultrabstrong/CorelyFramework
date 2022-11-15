using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using rm = UsefulUtilities.Resources.UI.ProgressDisplay;

namespace UsefulUtilities.UI
{
    /// <summary>
    /// Interaction logic for Progress.xaml
    /// </summary>
    public partial class ProgressDisplay : Window, INotifyPropertyChanged
    {
        #region Constructor / Initialization

        /// <summary>
        /// Construct this window
        /// </summary>
        public ProgressDisplay()
        {
            InitializeComponent();
            DataContext = this;
            Title = rm.progress;
            CancelTokenSource = new CancellationTokenSource();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Text to display
        /// </summary>
        public string DisplayText
        {
            get { return _displayText; }
            set
            {
                if (CancelTokenSource.IsCancellationRequested)
                {
                    value = $"{rm.cancelling}... {value}";
                }
                SetField(ref _displayText, value, nameof(DisplayText));
                this.Refresh();
            }
        }
        private string _displayText;

        /// <summary>
        /// Cancellation token source to cancel job with
        /// </summary>
        private CancellationTokenSource CancelTokenSource { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Throw exception if cancellation requested
        /// </summary>
        public void ThrowIfCancellationRequested()
        {
            CancelTokenSource.Token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Set display text
        /// </summary>
        /// <param name="text"></param>
        public void SetDisplayText(string text)
        {
            DisplayText = text;
        }

        /// <summary>
        /// Run the cancel token method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelToken();
        }

        /// <summary>
        /// Run the cancel token method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            CancelToken();
        }

        /// <summary>
        /// Cancel the cancellation token
        /// </summary>
        private void CancelToken()
        {
            if (!CancelTokenSource.IsCancellationRequested)
            {
                cancelButton.IsEnabled = false;
                CancelTokenSource.Cancel();
            }
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
