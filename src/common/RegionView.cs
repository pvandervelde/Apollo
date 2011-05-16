//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace Apollo.UI.Common
{
    /// <summary>
    /// A view that is shown in a region.
    /// </summary>
    [ExcludeFromCodeCoverage()]
    public class RegionView : UserControl, IRegionView
    {
        /// <summary>
        /// A dependency property that defines the title of the region.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = 
            DependencyProperty.Register("Title", typeof(string), typeof(RegionView), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// Indicates if the region has been shown or not.
        /// </summary>
        private bool m_HasShown;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionView"/> class.
        /// </summary>
        public RegionView()
        {
            Loaded += OnViewLoaded;
        }

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
                RaiseShown();
            }
        }

        /// <summary>
        /// Gets or sets the title of the region.
        /// </summary>
        /// <value>The title of the region.</value>
        public string Title
        {
            get 
            { 
                return (string)GetValue(TitleProperty); 
            }

            set 
            { 
                SetValue(TitleProperty, value); 
            }
        }

        /// <summary>
        /// Occurs when  the regrion is shown.
        /// </summary>
        public event EventHandler Shown;

        /// <summary>
        /// Occurs when the region is activated.
        /// </summary>
        public event EventHandler Activated;

        /// <summary>
        /// Occurs when the region is deactivated.
        /// </summary>
        public event EventHandler Deactivated;

        /// <summary>
        /// Occurs when the region is closing.
        /// </summary>
        public event CancelEventHandler Closing;

        /// <summary>
        /// Occurs when the region is closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Raises the <see cref="Shown"/> event.
        /// </summary>
        public void RaiseShown()
        {
            var handler = Shown;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="Closed"/> event.
        /// </summary>
        public void RaiseClosed()
        {
            var handler = Closed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="Closing"/> event.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the <see cref="Closing"/> event was cancelled; otherwise <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool RaiseClosing()
        {
            var handler = Closing;
            var args = new CancelEventArgs();
            if (handler != null)
            {
                handler(this, args);
            }

            return args.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="Activated"/> event.
        /// </summary>
        public void RaiseActivated()
        {
            var handler = Activated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="Deactivated"/> event.
        /// </summary>
        public void RaiseDeactivated()
        {
            var handler = Deactivated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}