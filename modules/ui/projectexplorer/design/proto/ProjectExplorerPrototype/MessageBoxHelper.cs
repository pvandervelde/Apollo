using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ProjectExplorerPrototype
{
    class MessageBoxHelper
    {
        #region Public Methods

        /// <summary>
        /// Show MessageBox
        /// </summary>
        /// <param name="text">text to show</param>
        /// <param name="caption">caption to show</param>
        public static void ShowMessageBox(string text, string caption)
        {
            MessageBox.Show(text, caption,
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Show Error MessageBox
        /// </summary>
        /// <param name="text">text to show</param>
        /// <param name="caption">caption to show</param>
        public static void ShowErrorBox(string text, string caption)
        {
            MessageBox.Show(text, caption,
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        #endregion
    }
}
