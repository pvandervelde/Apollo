//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines a number of constants related to files and file paths.
    /// </summary>
    public interface IFileConstants
    {
        /// <summary>
        /// Gets the extension for an assembly file.
        /// </summary>
        string AssemblyExtension
        {
            get;
        }

        /// <summary>
        /// Gets the extension for a log file.
        /// </summary>
        string LogExtension
        {
            get;
        }

        /// <summary>
        /// Gets the extension for a feedback file.
        /// </summary>
        string FeedbackReportExtension
        {
            get;
        }

        /// <summary>
        /// Returns the path for the directory in the AppData directory which contains
        /// all the product directories for the current company.
        /// </summary>
        /// <returns>
        /// The full path for the AppData directory for the current company.
        /// </returns>
        string CompanyCommonPath();

        /// <summary>
        /// Returns the path for the directory in the user specific AppData directory which contains
        /// all the product directories for the current company.
        /// </summary>
        /// <returns>
        /// The full path for the AppData directory for the current company.
        /// </returns>
        string CompanyUserPath();

        /// <summary>
        /// Returns the path for the directory where the global 
        /// settings for the product are written to.
        /// </summary>
        /// <returns>
        /// The full path for the directory where the global settings
        /// for the product are written to.
        /// </returns>
        string ProductSettingsUserPath();

        /// <summary>
        /// Returns the path for the directory where the log files are
        /// written to.
        /// </summary>
        /// <returns>
        /// The full path for the directory where the log files are written to.
        /// </returns>
        string LogPath();
    }
}
