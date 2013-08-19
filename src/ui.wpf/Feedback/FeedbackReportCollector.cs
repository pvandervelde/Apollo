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
        /// The object that holds the constant values that describe the application files and file paths.
        /// </summary>
        private readonly FileConstants m_FileConstants;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackReportCollector"/> class.
        /// </summary>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <param name="fileConstants">The object that holds the constant values that describe the application files and file paths.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileConstants"/> is <see langword="null" />.
        /// </exception>
        public FeedbackReportCollector(IFileSystem fileSystem, FileConstants fileConstants)
        {
            {
                Lokad.Enforce.Argument(() => fileSystem);
                Lokad.Enforce.Argument(() => fileConstants);
            }

            m_FileSystem = fileSystem;
            m_FileConstants = fileConstants;
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
                var path = m_FileConstants.CompanyUserPath();
                return m_FileSystem.Directory.GetFiles(
                    path,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "*.{0}",
                        m_FileConstants.FeedbackReportExtension),
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
