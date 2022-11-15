using UsefulUtilities.Data.Serialization;
using UsefulUtilities.Distribution;
using UsefulUtilities.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UsefulUtilities.TestWPF
{
    public partial class MainWindow : Window
    {
        #region Constructor / Initialization

        /// <summary>
        /// Construct and initialize this window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Finish setting up window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Set up split distribution
            distsettings.OnSave += Splitdist_OnSave;
            string splitsettingsfile = @"C:\ProgramData\SplitSettings\splitsettings.xml";
            if (File.Exists(splitsettingsfile))
            {
                string splitsettings = File.ReadAllText(splitsettingsfile);
                List<DistributionSettings> settings = XmlSerializer.DeSerialize<List<DistributionSettings>>(splitsettings);
                distsettings.SetSettings(settings);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Count for click me button
        /// </summary>
        public int Count { get; set; } = 0;

        #endregion

        #region Methods

        /// <summary>
        /// Save split distribution settings
        /// </summary>
        private void Splitdist_OnSave()
        {
            List<DistributionSettings> settings = distsettings.GetSettings();
            string splitsettingsxml = XmlSerializer.Serialize(settings);
            string splitsettingsfile = @"C:\ProgramData\SplitSettings\splitsettings.xml";
            File.WriteAllText(splitsettingsfile, splitsettingsxml);
            MessageBox.Show("Split distribution settings saved");
        }

        /// <summary>
        /// Click me counter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickme_Click(object sender, RoutedEventArgs e)
        {
            clickedcount.Text = $"Clicked count: {++Count}";
        }

        /// <summary>
        /// Test progress display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void testprogress_Click(object sender, RoutedEventArgs e)
        {
            ProgressDisplay progress = null;
            try
            {
                progress = AsyncWindow.ShowAsync<ProgressDisplay>();

                for (int i = 1; i < 7; i++)
                {
                    progress.DisplayText = "Processing task "+i+" of 6";
                    progress.ThrowIfCancellationRequested();
                    Thread.Sleep(1000);
                    progress.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                MessageDisplay message = AsyncWindow.ShowAsync<MessageDisplay>();
                message.DisplayText = "User cancelled operation";
                message.ShowCancel = true;
            }
            catch(Exception ex)
            {
                MessageDisplay message = AsyncWindow.ShowAsync<MessageDisplay>();
                message.DisplayText = "Error encountered: " + ex.Message;
            }
            finally
            {
                AsyncWindow.CloseAsync(progress);
            }
            MessageDisplay finalmessage = new MessageDisplay();
            finalmessage.DisplayText = "Finished";
            finalmessage.ShowCancel = true;
            finalmessage.ShowDialog();
            MessageBox.Show(finalmessage.Result.ToString());
        }

        #endregion

    }
}
