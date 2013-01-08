//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Input;

namespace Apollo.UI.Wpf.Views.Datasets
{
    /// <summary>
    /// Interaction logic for DatasetDetailView.xaml.
    /// </summary>
    public partial class DatasetDetailView : UserControl, IDatasetDetailView
    {
        /// <summary>
        /// The routed command that is used to put the dataset in edit mode or non-edit mode.
        /// </summary>
        private static readonly RoutedCommand s_EditModeCommand = new RoutedCommand();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetDetailView"/> class.
        /// </summary>
        public DatasetDetailView()
        {
            InitializeComponent();

            // Bind the edit mode command
            {
                var cb = new CommandBinding(s_EditModeCommand, CommandEditModeSwitchExecuted, CommandEditModeSwitchCanExecute);
                CommandBindings.Add(cb);
                switchEditModeButton.Command = s_EditModeCommand;
            }
        }

        /// <summary>
        /// Gets or sets the dataset model for the view.
        /// </summary>
        public DatasetDetailModel Model
        {
            get
            {
                return (DatasetDetailModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandEditModeSwitchCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            if ((Model != null) && Model.IsLoaded)
            {
                if (!Model.IsLocked)
                {
                    e.CanExecute = Model.SwitchDatasetToEditModeCommand.CanExecute(null);
                }
                else
                {
                    e.CanExecute = Model.SwitchDatasetToExecutingModeCommand.CanExecute(null);
                }
            }
            else
            {
                e.CanExecute = false;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandEditModeSwitchExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (!Model.IsLocked)
            {
                Model.SwitchDatasetToEditModeCommand.Execute(null);
            }
            else
            {
                Model.SwitchDatasetToExecutingModeCommand.Execute(null);
            }
        }
    }
}
