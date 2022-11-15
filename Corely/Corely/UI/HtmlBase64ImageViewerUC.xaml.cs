using System;
using System.Windows;
using System.Windows.Controls;

namespace Corely.UI
{
    /// <summary>
    /// Interaction logic for HtmlBase64ImageViewerUC.xaml
    /// </summary>
    public partial class HtmlBase64ImageViewerUC : UserControl
    {

        #region Constructor / Initialization

        /// <summary>
        /// Construct this control
        /// </summary>
        public HtmlBase64ImageViewerUC(string htmlContent)
        {
            InitializeComponent();
            HtmlContent = htmlContent;
            // Set the datacontext
            DataContext = this;
            // Display the HTML
            htmlDisplay.NavigateToString(HtmlContent);
            // Create window container
            Window = new Window()
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Height = 600,
                Width = 900,
                Content = this
            };
            Window.Closed += Window_Closed;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Parent window for this control
        /// </summary>
        public Window Window { get; internal set; }

        /// <summary>
        /// Html text to display
        /// </summary>
        public string HtmlContent { get; set; }

        /// <summary>
        /// ? Is window for this control closed
        /// </summary>
        public bool Closed { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// Show the parent window
        /// </summary>
        /// <param name="waitForClose"></param>
        public void Show(bool waitForClose)
        {
            if (waitForClose)
            {
                Window.ShowDialog();
            }
            else
            {
                Window.Show();
            }
        }

        /// <summary>
        /// Parent window closed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Closed = true;
        }

        #endregion

    }
}
