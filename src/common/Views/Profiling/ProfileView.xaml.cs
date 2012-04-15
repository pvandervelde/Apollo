//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;

namespace Apollo.UI.Common.Views.Profiling
{
    /// <summary>
    /// Interaction logic for ProfileView.xaml.
    /// </summary>
    public partial class ProfileView : UserControl, IProfileView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileView"/> class.
        /// </summary>
        public ProfileView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public ProfileModel Model
        {
            get
            {
                return (ProfileModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }

        private void OnShowProfileResultsButtonClick(object sender, RoutedEventArgs e)
        {
            profileResultsPopup.IsOpen = true;
        }

        private void OnProfileResultsPopupClosed(object sender, EventArgs e)
        {
            ClearControls();
        }

        private void ClearControls()
        {
            profileResultsPopup.IsOpen = false;
        }

        private void OnNavigateForwardsButtonClick(object sender, RoutedEventArgs e)
        {
            Model.Results.MoveCurrentToNext();
        }

        private void OnNavigateBackwardsButtonClick(object sender, RoutedEventArgs e)
        {
            Model.Results.MoveCurrentToPrevious();
        }
    }
}
