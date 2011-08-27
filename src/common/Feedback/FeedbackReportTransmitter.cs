//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using Apollo.UI.Common.Properties;
using Lokad;
using NSarrac.Framework;

namespace Apollo.UI.Common.Feedback
{
    /// <summary>
    /// Sends feedback reports to a remote server.
    /// </summary>
    public sealed class FeedbackReportTransmitter : ISendFeedbackReports
    {
        /// <summary>
        /// The object that sends the reports to the remote server.
        /// </summary>
        private readonly IReportSender m_Sender;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackReportTransmitter"/> class.
        /// </summary>
        /// <param name="sender">The object that sends the reports to the remote server.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sender"/> is <see langword="null" />.
        /// </exception>
        public FeedbackReportTransmitter(IReportSender sender)
        {
            {
                Enforce.Argument(() => sender);
            }

            m_Sender = sender;
        }

        /// <summary>
        /// Sends the given report to a remote report server.
        /// </summary>
        /// <param name="reportStream">The stream that contains the feedback report.</param>
        public void Send(Stream reportStream)
        {
            try
            {
                m_Sender.SendReport(reportStream);
            }
            catch (CouldNotConnectToTheRemoteServiceException e)
            {
                throw new FailedToSendFeedbackReportException(
                    Resources.Exceptions_Messages_FailedToSendFeedbackReport, 
                    e);
            }
        }
    }
}
