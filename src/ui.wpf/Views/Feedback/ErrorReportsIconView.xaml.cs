//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Apollo.UI.Wpf.Views.Feedback
{
    /// <summary>
    /// Interaction logic for ErrorReportIconView.xaml.
    /// </summary>
    public partial class ErrorReportsIconView : UserControl, IErrorReportsView
    {
        /// <summary>
        /// The routed command used to send the error reports to the server.
        /// </summary>
        private static readonly RoutedCommand s_SendReportsCommand = new RoutedCommand();

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportsIconView"/> class.
        /// </summary>
        public ErrorReportsIconView()
        {
            InitializeComponent();

            // Bind the send error reports button
            {
                var cb = new CommandBinding(s_SendReportsCommand, CommandSendReportsExecuted, CommandSendReportsCanExecute);
                CommandBindings.Add(cb);
                sendReportsButton.Command = s_SendReportsCommand;
            }
        }

        /// <summary>
        /// Gets or sets the dataset model for the view.
        /// </summary>
        public ErrorReportsModel Model
        {
            get
            {
                return (ErrorReportsModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandSendReportsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var items = from FeedbackFileModel report in errorReportsListBox.SelectedItems
                        select new FileInfo(report.Path);

            e.Handled = true;
            e.CanExecute = Model != null ? items.Any() : false;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandSendReportsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var items = from FeedbackFileModel report in errorReportsListBox.SelectedItems
                        select report;

            e.Handled = true;
            Model.SendReports(items);
        }

        private void OnErrorReportButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            errorReportPopup.IsOpen = true;
        }

        private void OnErrorReportCancelButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            errorReportPopup.IsOpen = false;
        }

        private void OnSelectAllCheckBoxChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            errorReportsListBox.SelectAll();
        }

        private void OnSelectAllCheckBoxUnchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            errorReportsListBox.UnselectAll();
        }
    }
}
