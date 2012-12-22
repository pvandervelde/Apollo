//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

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
        /// Creates a uniquely named, zero-byte temporary file on disk and returns the full path of that file.
        /// </summary>
        /// <returns>The full path of the temporary file.</returns>
        string GetTempFileName();

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

        /// <summary>
        /// Opens a stream to the given file with the provided access mode and creation mode.
        /// </summary>
        /// <param name="path">The full path to the file that should be opened.</param>
        /// <param name="fileMode">A flag indicating how the file should be opened.</param>
        /// <param name="fileAccess">A flag indicating how the file should be shared while it is open.</param>
        /// <returns>An file stream object.</returns>
        IVirtualFileStream Open(string path, FileMode fileMode, FileAccess fileAccess);
    }
}
