//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows;

namespace Apollo.UI.Common
{
    /// <summary>
    /// A view that is encapsulated in a window.
    /// </summary>
    public class WindowView : Window, IStandardView
    {
        /// <summary>
        /// Indicates if the view has been shown.
        /// </summary>
        private bool m_HasShown;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowView"/> class.
        /// </summary>
        public WindowView()
        {
            Loaded += OnViewLoaded;
        }

        /// <summary>
        /// Occurs when the view is shown.
        /// </summary>
        public event EventHandler Shown;

        /// <summary>
        /// Called when the view is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            if (!m_HasShown)
            {
                m_HasShown = true;

                var handler = Shown;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }
    }
}