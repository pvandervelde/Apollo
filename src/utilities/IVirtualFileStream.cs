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
    /// Defines the interface for objects that virtualize a file stream.
    /// </summary>
    public interface IVirtualFileStream : IDisposable
    {
        /// <summary>
        /// Copies the given stream.
        /// </summary>
        /// <param name="stream">The stream to copy.</param>
        void CopyFrom(Stream stream);
    }
}
