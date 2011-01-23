//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utils
{
    /// <summary>
    /// Defines a number of constants related to files and file paths.
    /// </summary>
    public interface IFileConstants
    {
        /// <summary>
        /// Gets the extension for an assembly file.
        /// </summary>
        /// <value>The extension for an assembly file.</value>
        string AssemblyExtension
        {
            get;
        }

        /// <summary>
        /// Gets the extension for a log file.
        /// </summary>
        /// <value>The extension for a log file.</value>
        string LogExtension
        {
            get;
        }

        /// <summary>
        /// Returns the path for the directory where the global 
        /// settings for the product are written to.
        /// </summary>
        /// <returns>
        /// The full path for the directory where the global settings
        /// for the product are written to.
        /// </returns>
        string ProductSettingsPath();

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
