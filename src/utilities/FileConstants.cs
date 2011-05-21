//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Lokad;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines a set of values related to files and file paths.
    /// </summary>
    [Serializable]
    public sealed class FileConstants : IFileConstants
    {
        /// <summary>
        /// The object that stores constant values for the application.
        /// </summary>
        private readonly IApplicationConstants m_Constants;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConstants"/> class.
        /// </summary>
        /// <param name="constants">The object that stores constant values for the application.</param>
        public FileConstants(IApplicationConstants constants)
        {
            {
                Enforce.Argument(() => constants);
            }

            m_Constants = constants;
        }

        #region Implementation of IFileConstants

        /// <summary>
        /// Gets the extension for an assembly file.
        /// </summary>
        /// <value>The extension for an assembly file.</value>
        public string AssemblyExtension
        {
            [ExcludeFromCodeCoverage]
            get
            {
                return ".dll";
            }
        }

        /// <summary>
        /// Gets the extension for a log file.
        /// </summary>
        /// <value>The extension for a log file.</value>
        public string LogExtension
        {
            [ExcludeFromCodeCoverage]
            get
            {
                return ".log";
            }
        }

        /// <summary>
        /// Returns the path for the directory where the global 
        /// settings for the product are written to.
        /// </summary>
        /// <returns>
        /// The full path for the directory where the global settings
        /// for the product are written to.
        /// </returns>
        public string ProductSettingsPath()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var companyDirectory = Path.Combine(appDataDir, m_Constants.CompanyName);
            var productDirectory = Path.Combine(companyDirectory, m_Constants.ApplicationName);
            var versionDirectory = Path.Combine(productDirectory, m_Constants.ApplicationCompatibilityVersion.ToString(2));

            return versionDirectory;
        }

        /// <summary>
        /// Returns the path for the directory where the log files are
        /// written to.
        /// </summary>
        /// <returns>
        /// The full path for the directory where the log files are written to.
        /// </returns>
        public string LogPath()
        {
            var versionDirectory = ProductSettingsPath();
            var logDirectory = Path.Combine(versionDirectory, "logs");

            return logDirectory;
        }

        #endregion
    }
}
