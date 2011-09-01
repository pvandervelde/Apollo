//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
        /// <param name="feedbackReport">A stream that contains a feedback report.</param>
        /// <returns>
        ///     <see langword="true"/> if the existing project can be closed; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanSendReport(ISendFeedbackReports feedbackSender, Stream feedbackReport)
        {
            return (feedbackSender != null) && (feedbackReport != null) && (feedbackReport != null);
        }
        
        /// <summary>
        /// Called when a report should be send to the remote server.
        /// </summary>
        /// <param name="feedbackSender">The object that sends the report to the remote server.</param>
        /// <param name="feedbackReport">A stream that contains a feedback report.</param>
        private static void OnSendReport(ISendFeedbackReports feedbackSender, Stream feedbackReport)
        {
            if (feedbackSender == null)
            {
                return;
            }

            if (feedbackReport == null)
            {
                return;
            }

            try
            {
                feedbackSender.Send(feedbackReport);
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SendFeedbackReportCommand"/> class.
        /// </summary>
        /// <param name="feedbackSender">
        /// The object that is responsible for sending reports to the remote server.
        /// </param>
        public SendFeedbackReportCommand(ISendFeedbackReports feedbackSender)
            : base(
                obj => OnSendReport(feedbackSender, obj as Stream), 
                obj => CanSendReport(feedbackSender, obj as Stream))
        { 
        }
    }
}
