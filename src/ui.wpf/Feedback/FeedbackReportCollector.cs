//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Apollo.Utilities;

namespace Apollo.UI.Wpf.Feedback
{
    /// <summary>
    /// Collect the feedback reports which are stored on the disk.
    /// </summary>
    public sealed class FeedbackReportCollector : ICollectFeedbackReports
    {
        /// <summary>
        /// The object that provides access to the file system.
        /// </summary>
        private readonly IFileSystem m_FileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackReportCollector"/> class.
        /// </summary>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        public FeedbackReportCollector(IFileSystem fileSystem)
        {
            {
                Lokad.Enforce.Argument(() => fileSystem);
            }

            m_FileSystem = fileSystem;
        }

        /// <summary>
        /// Returns a collection containing the file information for the stored 
        /// feedback reports.
        /// </summary>
        /// <returns>
        ///     The collection that contains the file information.
        /// </returns>
        public IEnumerable<FileInfo> LocateFeedbackReports()
        {
            var files = from file in FeedbackReportsFilePaths()
                        select new FileInfo(file);

            return files;
        }

        private IEnumerable<string> FeedbackReportsFilePaths()
        {
            try
            {
                var path = FileConstants.CompanyUserPath();
                return m_FileSystem.Directory.GetFiles(
                    path,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "*.{0}",
                        FileConstants.FeedbackReportExtension),
                    SearchOption.AllDirectories);
            }
            catch (UnauthorizedAccessException)
            {
                return new List<string>();
            }
            catch (IOException)
            {
                return new List<string>();
            }
        }
    }
}
