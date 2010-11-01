//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;

namespace Apollo.ProjectExplorer.Views.About
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml.
    /// </summary>
    internal partial class AboutWindow : IAboutView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutWindow"/> class.
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
        }

        #region Implementation of IView<AboutModel>

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public AboutModel Model
        {
            get
            {
                return (AboutModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }

        #endregion

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Close();
        }
    }
}
