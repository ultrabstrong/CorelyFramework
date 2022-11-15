using Corely.UI;

namespace Corely.Core
{
    public static class Message
    {
        /// <summary>
        /// Show message box synchronously
        /// </summary>
        /// <param name="text"></param>
        public static void Show(string text)
        {
            MessageDisplay display = new MessageDisplay();
            display.DisplayText = text;
            display.ShowDialog();
        }

        /// <summary>
        /// Show message box asynchronously
        /// </summary>
        /// <param name="text"></param>
        public static void ShowAsync(string text)
        {
            MessageDisplay display = AsyncWindow.ShowAsync<MessageDisplay>();
            display.DisplayText = text;
        }
    }
}
