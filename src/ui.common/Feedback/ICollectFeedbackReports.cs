//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;

namespace Apollo.UI.Common.Feedback
{
    /// <summary>
    /// Defines the interface for objects that collect stored feedback reports.
    /// </summary>
    public interface ICollectFeedbackReports
    {
        /// <summary>
        /// Returns a collection containing the file information for the stored 
        /// feedback reports.
        /// </summary>
        /// <returns>
        ///     The collection that contains the file information.
        /// </returns>
        IEnumerable<FileInfo> LocateFeedbackReports();
    }
}
