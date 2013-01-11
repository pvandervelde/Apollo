//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;

namespace Apollo.UI.Wpf.Feedback
{
    /// <summary>
    /// Defines the interface for objects that send feedback reports to a remote server.
    /// </summary>
    public interface ISendFeedbackReports
    {
        /// <summary>
        /// Sends the given report to a remote report server.
        /// </summary>
        /// <param name="reportStream">The stream that contains the feedback report.</param>
        void Send(Stream reportStream);
    }
}
