// Code based on:
// Codeproject article: Fun with Physics (http://www.codeproject.com/KB/WPF/PhysicsFun.aspx)
// By: Sacha Barber (http://sachabarber.net/)
//     Fredrik Bornander (http://fredrik.bornander.googlepages.com/)
// The code in the article is licensed under the Code Project Open License 

using System.Windows;

namespace Apollo.Ui.Common.Misc
{
    /// <summary>
    /// A simple MessageBox helper
    /// </summary>
    public sealed class MessageBoxHelper
    {
        /// <summary>
        /// Shows a message box with an error message. Displays an Error icon and an Ok button.
        /// </summary>
        /// <param name="caption">The caption to show</param>
        /// <param name="text">The text to show</param>
        public static void ShowErrorBox(string caption, string text)
        {
            ShowMessageBox(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows a message box with the specified information.
        /// </summary>
        /// <param name="caption">The caption for the dialog.</param>
        /// <param name="text">The text that should be shown in the main area of the dialog.</param>
        /// <param name="buttons">The buttons that should be shown on the dialog.</param>
        /// <param name="icon">The icon that should be shown on the dialog.</param>
        /// <returns>
        /// The dialog result from the message box.
        /// </returns>
        public static MessageBoxResult ShowMessageBox(string caption, string text, MessageBoxButton buttons, MessageBoxImage icon)
        {
            return MessageBox.Show(text, caption, buttons, icon);
        }
    }
}
