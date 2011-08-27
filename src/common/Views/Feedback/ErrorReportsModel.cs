//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Apollo.UI.Common.Feedback;

namespace Apollo.UI.Common.Views.Feedback
{
    /// <summary>
    /// Defines the model for a collection of error reports.
    /// </summary>
    public sealed class ErrorReportsModel : Model
    {
        /// <summary>
        /// The collection that contains the reports.
        /// </summary>
        private readonly ObservableCollection<FeedbackReportModel> m_Reports
            = new ObservableCollection<FeedbackReportModel>();

        /// <summary>
        /// The object that collects the feedback reports that have been stored on the
        /// disk.
        /// </summary>
        private readonly ICollectFeedbackReports m_FeedbackCollector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportsModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="sendReportsCommand">The command that is used to send the feedback reports to the remote server.</param>
        /// <param name="feedbackCollector">The object that collects the feedback reports that have been stored on the disk.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendReportsCommand"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="feedbackCollector"/> is <see langword="null" />.
        /// </exception>
        public ErrorReportsModel(IContextAware context, ICommand sendReportsCommand, ICollectFeedbackReports feedbackCollector)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => sendReportsCommand);
                Lokad.Enforce.Argument(() => feedbackCollector);
            }

            SendReportsCommand = sendReportsCommand;
            m_FeedbackCollector = feedbackCollector;

            RelocateReports();
        }

        /// <summary>
        /// Initiates the search for feedback reports which are stored on the disk.
        /// </summary>
        public void RelocateReports()
        {
            m_Reports.Clear();
            Action<FileInfo> action =
                item =>
                {
                    m_Reports.Add(new FeedbackReportModel(item.FullName, item.CreationTime, InternalContext));
                };

            var task = Task.Factory.StartNew(
                () =>
                {
                    var list = m_FeedbackCollector.LocateFeedbackReports();
                    foreach (var item in list)
                    {
                        if (InternalContext.IsSynchronized)
                        {
                            action(item);
                        }
                        else
                        {
                            InternalContext.Invoke(() => action(item));
                        }
                    }

                    Notify(() => HasErrorReports);
                });
        }

        /// <summary>
        /// Gets a value indicating whether there are any error reports.
        /// </summary>
        public bool HasErrorReports
        {
            get
            {
                return Reports.Count > 0;
            }
        }

        /// <summary>
        /// Gets the collection of error reports.
        /// </summary>
        public ObservableCollection<FeedbackReportModel> Reports
        {
            get
            {
                return m_Reports;
            }
        }

        /// <summary>
        /// Gets the command that can be used to send a given set of reports 
        /// to the server.
        /// </summary>
        public ICommand SendReportsCommand
        {
            get;
            private set;
        }
    }
}
