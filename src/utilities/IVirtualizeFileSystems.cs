//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the interface for objects that virtualize the file stystem.
    /// </summary>
    /// <remarks>
    /// This class is mainly used to make the testing of interactions with the file system easier / possible.
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Virtualize",
        Justification = "US-English spelling of making something virtual.")]
    public interface IVirtualizeFileSystems
    {
        /// <summary>
        /// Returns a value indicating if the file exists or not.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        /// <returns>
        /// <see langword="true" /> if the file exists; otherwise <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool DoesFileExist(string filePath);

        /// <summary>
        /// Gets the last time the file was written to in UTC time.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        /// <returns>The last time the file was written to in UTC time.</returns>
        DateTimeOffset FileLastWriteTimeUtc(string filePath);

        /// <summary>
        /// Gets the files in the given directory.
        /// </summary>
        /// <param name="directoryPath">The full path to the directory.</param>
        /// <param name="searchPattern">The search string to match against the files in the path.</param>
        /// <param name="searchSubdirectories">Indicates if the sub-directories of the current directory should be searched for files.</param>
        /// <returns>
        /// The collection of files that are contained in the directory and possible sub-directories.
        /// </returns>
        IEnumerable<string> GetFilesInDirectory(string directoryPath, string searchPattern, bool searchSubdirectories);
    }
}
