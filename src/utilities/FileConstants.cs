//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines a set of values related to files and file paths.
    /// </summary>
    public static class FileConstants
    {
        /// <summary>
        /// Gets the extension for an assembly file.
        /// </summary>
        /// <value>The extension for an assembly file.</value>
        public static string AssemblyExtension
        {
            get
            {
                return "dll";
            }
        }

        /// <summary>
        /// Gets the extension for a log file.
        /// </summary>
        /// <value>The extension for a log file.</value>
        public static string LogExtension
        {
            get
            {
                return "log";
            }
        }

        /// <summary>
        /// Gets the extension for a feedback file.
        /// </summary>
        public static string FeedbackReportExtension
        {
            get
            {
                return "nsdump";
            }
        }

        /// <summary>
        /// Returns the path for the directory in the AppData directory which contains
        /// all the product directories for the current company.
        /// </summary>
        /// <returns>
        /// The full path for the AppData directory for the current company.
        /// </returns>
        public static string CompanyCommonPath()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var companyDirectory = Path.Combine(appDataDir, ApplicationConstants.CompanyName);

            return companyDirectory;
        }

        /// <summary>
        /// Returns the path for the directory in the user specific AppData directory which contains
        /// all the product directories for the current company.
        /// </summary>
        /// <returns>
        /// The full path for the AppData directory for the current company.
        /// </returns>
        public static string CompanyUserPath()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var companyDirectory = Path.Combine(appDataDir, ApplicationConstants.CompanyName);

            return companyDirectory;
        }

        /// <summary>
        /// Returns the path for the directory where the global
        /// settings for the product are written to.
        /// </summary>
        /// <returns>
        /// The full path for the directory where the global settings
        /// for the product are written to.
        /// </returns>
        public static string ProductSettingsCommonPath()
        {
            var companyDirectory = CompanyCommonPath();
            var productDirectory = Path.Combine(companyDirectory, ApplicationConstants.ApplicationName);
            var versionDirectory = Path.Combine(productDirectory, ApplicationConstants.ApplicationCompatibilityVersion.ToString(2));

            return versionDirectory;
        }

        /// <summary>
        /// Returns the path for the directory where the global 
        /// settings for the product are written to.
        /// </summary>
        /// <returns>
        /// The full path for the directory where the global settings
        /// for the product are written to.
        /// </returns>
        public static string ProductSettingsUserPath()
        {
            var companyDirectory = CompanyUserPath();
            var productDirectory = Path.Combine(companyDirectory, ApplicationConstants.ApplicationName);
            var versionDirectory = Path.Combine(productDirectory, ApplicationConstants.ApplicationCompatibilityVersion.ToString(2));

            return versionDirectory;
        }

        /// <summary>
        /// Returns the path for the directory where the log files are
        /// written to.
        /// </summary>
        /// <returns>
        /// The full path for the directory where the log files are written to.
        /// </returns>
        public static string LogPath()
        {
            var versionDirectory = ProductSettingsUserPath();
            var logDirectory = Path.Combine(versionDirectory, "logs");

            return logDirectory;
        }
    }
}
