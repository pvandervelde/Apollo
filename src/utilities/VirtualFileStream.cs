//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the methods necessary to virtualize a file stream.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class VirtualFileStream : IVirtualFileStream
    {
        /// <summary>
        /// The internal stream that does the actual work.
        /// </summary>
        private readonly FileStream m_Stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualFileStream"/> class.
        /// </summary>
        /// <param name="stream">The internal stream.</param>
        public VirtualFileStream(FileStream stream)
        {
            {
                Lokad.Enforce.Argument(() => stream);
            }

            m_Stream = stream;
        }

        /// <summary>
        /// Copies the given stream.
        /// </summary>
        /// <param name="stream">The stream to copy.</param>
        public void CopyFrom(Stream stream)
        {
            stream.CopyTo(m_Stream);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_Stream.Dispose();
        }
    }
}
