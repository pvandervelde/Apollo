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
    /// Defines methods for dealing with persistent storage.
    /// </summary>
    public interface IPersistenceInformation
    {
        /// <summary>
        /// Gets the version of the stored information.
        /// </summary>
        Version Version
        {
            get;
        }

        /// <summary>
        /// Returns the size of the stored information in bytes.
        /// </summary>
        /// <returns>
        /// The size of the stored information in bytes.
        /// </returns>
        long StoredSizeInBytes();

        /// <summary>
        /// Returns a <see cref="FileInfo"/> object that points to the stored data in the file system.
        /// </summary>
        /// <returns>
        ///     An object that points to the correct file in the file system.
        /// </returns>
        FileInfo AsFile();

        /// <summary>
        /// Returns a <see cref="Stream"/> object that points to the stored data.
        /// </summary>
        /// <returns>
        ///     An object that points to the data as a streamable object.
        /// </returns>
        Stream AsStream();
    }
}
