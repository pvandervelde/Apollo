//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines an empty <see cref="IPersistenceInformation"/> object which is used to
    /// create new empty datasets.
    /// </summary>
    public sealed class NullPersistenceInformation : IPersistenceInformation
    {
        /// <summary>
        /// Gets the version of the stored information.
        /// </summary>
        public Version Version
        {
            get
            {
                // For now we return the version of the assembly. At some point
                // we'll need to return the most recent version of the file format
                // or more specifically, we'll need to be able to generate the data
                // for an empty dataset and get the correct version etc..
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        /// <summary>
        /// Returns a <see cref="FileInfo"/> object that points to the stored data in the file system.
        /// </summary>
        /// <returns>
        ///     An object that points to the correct file in the file system.
        /// </returns>
        public FileInfo AsFile()
        {
            return new FileInfo(Path.GetTempFileName());
        }

        /// <summary>
        /// Returns a <see cref="Stream"/> object that points to the stored data.
        /// </summary>
        /// <returns>
        ///     An object that points to the data as a streamable object.
        /// </returns>
        public Stream AsStream()
        {
            return new MemoryStream();
        }
    }
}
