//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using System.Windows.Input;
using Apollo.UI.Wpf.Feedback;

namespace Apollo.UI.Wpf.Views.Feedback
{
    /// <summary>
    /// Defines the model for a collection of error reports.
    /// </summary>
    public sealed class ErrorReportsModel : Model
    {
        /// <summary>
        /// The collection that contains the reports.
        /// </summary>
        private readonly ObservableCollection<FeedbackFileModel> m_Reports
            = new ObservableCollection<FeedbackFileModel>();

        /// <summary>
        /// The object that collects the feedback reports that have been stored on the
        /// disk.
        /// </summary>
        private readonly ICollectFeedbackReports m_FeedbackCollector;

        /// <summary>
        /// The object that provides access to the file system.
        /// </summary>
        private readonly IFileSystem m_FileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportsModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="sendReportsCommand">The command that is used to send the feedback reports to the remote server.</param>
        /// <param name="feedbackCollector">The object that collects the feedback reports that have been stored on the disk.</param>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendReportsCommand"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="feedbackCollector"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        public ErrorReportsModel(
            IContextAware context, 
            ICommand sendReportsCommand, 
            ICollectFeedbackReports feedbackCollector,
            IFileSystem fileSystem)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => sendReportsCommand);
                Lokad.Enforce.Argument(() => feedbackCollector);
                Lokad.Enforce.Argument(() => fileSystem);
            }

            SendReportsCommand = sendReportsCommand;
            m_FeedbackCollector = feedbackCollector;
            m_FileSystem = fileSystem;

            RelocateReports();
        }

        /// <summary>
        /// Initiates the search for feedback reports which are stored on the disk.
        /// </summary>
        private void RelocateReports()
        {
            Task.Factory.StartNew(
                () =>
                {
                    m_Reports.Clear();

                    var list = m_FeedbackCollector.LocateFeedbackReports();
                    foreach (var item in list)
                    {
                        m_Reports.Add(new FeedbackFileModel(InternalContext, item.FullName, item.CreationTime));
                    }

                    Notify(() => HasErrorReports);
                });
        }

        /// <summary>
        /// Sends the reports to the remote server.
        /// </summary>
        /// <param name="reportFiles">The collection that holds the reports that should be send.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="reportFiles"/> is <see langword="null" />.
        /// </exception>
        public void SendReports(IEnumerable<FeedbackFileModel> reportFiles)
        {
            {
                Lokad.Enforce.Argument(() => reportFiles);
            }

            foreach (var report in reportFiles)
            {
                try
                {
                    using (var stream = m_FileSystem.File.Open(report.Path, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        SendReportsCommand.Execute(stream);
                    }

                    m_FileSystem.File.Delete(report.Path);
                }
                catch (UnauthorizedAccessException)
                {
                    // Ignore it and move on.
                }
                catch (IOException)
                {
                    // Ignore it and move on.
                }
                catch (FailedToSendFeedbackReportException)
                {
                    // Ignore it and move on.
                }
            }

            RelocateReports();
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
        public ObservableCollection<FeedbackFileModel> Reports
        {
            get
            {
                return m_Reports;
            }
        }

        /// <summary>
        /// Gets or sets the command that can be used to send a given set of reports 
        /// to the server.
        /// </summary>
        private ICommand SendReportsCommand
        {
            get;
            set;
        }
    }
}
