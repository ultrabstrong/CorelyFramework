using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace UsefulUtilities.UI
{
    public static class AsyncWindow
    {
        /// <summary>
        /// Display window asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ShowAsync<T>(params object[] args) where T : Window
        {
            T window = null;
            bool loaded = false;
            // Create a thread
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                // Create our context, and install it:
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                // Create window from provided type
                window = (T)Activator.CreateInstance(typeof(T), args);
                // Set notification that window has loaded
                window.Loaded += (s, e) => { loaded = true; };
                // When the window closes, shut down the dispatcher
                window.Closed += (s, e) => { Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background); };
                window.Show();
                // Start the Dispatcher Processing
                Dispatcher.Run();
            }));
            // Set the apartment state
            newWindowThread.SetApartmentState(ApartmentState.STA);
            // Make the thread a background thread
            newWindowThread.IsBackground = true;
            // Start the thread
            newWindowThread.Start();
            // Wait for window to load
            while (loaded == false)
            {
                Thread.Sleep(100);
            }
            // Return window on new thread
            return window;
        }

        /// <summary>
        /// Close window asynchronously
        /// </summary>
        /// <param name="window"></param>
        public static void CloseAsync(Window window)
        {
            if (window != null)
            {
                bool closed = false;
                // Set flag that window has closed
                window.Closed += (s, e) => { closed = true; };
                // Close the window
                window.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => { window.Close(); }));
                // Wait for window to close
                while (closed == false)
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
