﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Input;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// Interaction logic for DatasetVertexView.xaml.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class DatasetVertexView : UserControl, IDatasetView
    {
        /// <summary>
        /// The routed command used to create a new dataset.
        /// </summary>
        private static readonly RoutedCommand s_NewDatasetCommand = new RoutedCommand();

        /// <summary>
        /// The routed command used to remove a dataset.
        /// </summary>
        private static readonly RoutedCommand s_RemoveDatasetCommand = new RoutedCommand();

        /// <summary>
        /// The routed command that used to load and unload a dataset.
        /// </summary>
        private static readonly RoutedCommand s_LoadOrUnloadDatasetCommand = new RoutedCommand();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetVertexView"/> class.
        /// </summary>
        public DatasetVertexView()
        {
            InitializeComponent();

            // Bind the new dataset command
            {
                var cb = new CommandBinding(s_NewDatasetCommand, CommandNewDatasetExecuted, CommandNewDatasetCanExecute);
                CommandBindings.Add(cb);
                addChildDatasetButton.Command = s_NewDatasetCommand;
            }

            // Bind the delete dataset command
            {
                var cb = new CommandBinding(s_RemoveDatasetCommand, CommandDeleteDatasetExecuted, CommandDeleteDatasetCanExecute);
                CommandBindings.Add(cb);
                deleteDatasetButton.Command = s_RemoveDatasetCommand;
            }

            // Bind the load / unload command
            {
                var cb = new CommandBinding(s_LoadOrUnloadDatasetCommand, CommandLoadUnloadDatasetExecuted, CommandLoadUnloadDatasetCanExecute);
                CommandBindings.Add(cb);
                loadOrUnloadDatasetButton.Command = s_LoadOrUnloadDatasetCommand;
            }
        }

        /// <summary>
        /// Gets or sets the dataset model for the view.
        /// </summary>
        public DatasetModel Model
        {
            get
            {
                return (DatasetModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandNewDatasetCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model != null ? Model.NewChildDatasetCommand.CanExecute(null) : false;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandNewDatasetExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.NewChildDatasetCommand.Execute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandDeleteDatasetCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model != null ? Model.DeleteDatasetCommand.CanExecute(null) : false;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandDeleteDatasetExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.DeleteDatasetCommand.Execute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandLoadUnloadDatasetCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            if (Model != null)
            {
                if (!Model.IsLoaded)
                {
                    e.CanExecute = Model.LoadDatasetCommand.CanExecute(null);
                }
                else
                {
                    e.CanExecute = Model.UnloadDatasetCommand.CanExecute(null);
                }
            }
            else
            {
                e.CanExecute = false;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandLoadUnloadDatasetExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (!Model.IsLoaded)
            {
                // Should do something like this: http://presentationlayer.wordpress.com/2011/05/24/wpf-overlay-message-view-controller/
                // For an overlay ...
                Model.LoadDatasetCommand.Execute(null);
            }
            else 
            {
                Model.UnloadDatasetCommand.Execute(null);
            }
        }
    }
}