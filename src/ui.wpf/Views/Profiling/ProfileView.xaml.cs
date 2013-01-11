//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Apollo.UI.Wpf.Views.Profiling
{
    /// <summary>
    /// Interaction logic for ProfileView.xaml.
    /// </summary>
    public partial class ProfileView : UserControl, IProfileView
    {
        /// <summary>
        /// The routed command used move 1 report back in the reports collection.
        /// </summary>
        private static readonly RoutedCommand s_NavigateBackwardsCommand = new RoutedCommand();

        /// <summary>
        /// The routed command used to move 1 report forward in the reports collection.
        /// </summary>
        private static readonly RoutedCommand s_NavigateForwardsCommand = new RoutedCommand();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileView"/> class.
        /// </summary>
        public ProfileView()
        {
            InitializeComponent();

            // Bind the navigate backward button
            {
                var cb = new CommandBinding(s_NavigateBackwardsCommand, CommandNavigateBackwardsExecuted, CommandNavigateBackwardsCanExecute);
                CommandBindings.Add(cb);
                navigateBackwardsButton.Command = s_NavigateBackwardsCommand;
            }

            // Bind the navigate forward button
            {
                var cb = new CommandBinding(s_NavigateForwardsCommand, CommandNavigateForwardsExecuted, CommandNavigateForwardsCanExecute);
                CommandBindings.Add(cb);
                navigateForwardsButton.Command = s_NavigateForwardsCommand;
            }
        }

        private void CommandNavigateBackwardsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = (Model != null) && !Model.Results.IsCurrentBeforeFirst;
        }

        private void CommandNavigateBackwardsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (Model != null)
            {
                Model.Results.MoveCurrentToPrevious();
            }
        }

        private void CommandNavigateForwardsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = (Model != null) && !Model.Results.IsCurrentAfterLast;
        }

        private void CommandNavigateForwardsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (Model != null)
            {
                Model.Results.MoveCurrentToNext();
            }
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
            if (Model != null)
            {
                if (Model.Results.IsCurrentBeforeFirst)
                {
                    Model.Results.MoveCurrentToFirst();
                }

                if (Model.Results.IsCurrentAfterLast)
                {
                    Model.Results.MoveCurrentToLast();
                }
            }

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
    }
}
