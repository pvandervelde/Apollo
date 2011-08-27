//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Apollo.UI.Common.Feedback;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Common.Commands
{
    /// <summary>
    /// Handles the sending of feedback reports to a remote server.
    /// </summary>
    public sealed class SendFeedbackReportCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if a report can be send to the remote server.
        /// </summary>
        /// <param name="feedbackSender">The object that sends the report to the remote server.</param>
        /// <param name="feedbackReports">A collection that contains the file information for the feedback reports.</param>
        /// <returns>
        ///     <see langword="true"/> if the existing project can be closed; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanSendReport(ISendFeedbackReports feedbackSender, IEnumerable<FileInfo> feedbackReports)
        {
            return (feedbackSender != null) && (feedbackReports != null) && feedbackReports.Any();
        }
        
        /// <summary>
        /// Called when a report should be send to the remote server.
        /// </summary>
        /// <param name="feedbackSender">The object that sends the report to the remote server.</param>
        /// <param name="feedbackReports">The stream that contains the report that should be send.</param>
        private static void OnSendReport(ISendFeedbackReports feedbackSender, IEnumerable<FileInfo> feedbackReports)
        {
            if (feedbackSender == null)
            {
                return;
            }

            if ((feedbackReports == null) && feedbackReports.Any())
            {
                return;
            }

            foreach (var report in feedbackReports)
            {
                try
                {
                    using (var stream = report.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        feedbackSender.Send(stream);
                    }

                    report.Delete();
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendFeedbackReportCommand"/> class.
        /// </summary>
        /// <param name="feedbackSender">
        /// The object that is responsible for sending reports to the remote server.
        /// </param>
        public SendFeedbackReportCommand(ISendFeedbackReports feedbackSender)
            : base(
                obj => OnSendReport(feedbackSender, obj as IEnumerable<FileInfo>), 
                obj => CanSendReport(feedbackSender, obj as IEnumerable<FileInfo>))
        { 
        }
    }
}
