//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Net;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods for storing information about the way a stream should 
    /// be transferred over a stream connection.
    /// </summary>
    [Serializable]
    internal abstract class StreamTransferInformation
    {
        /// <summary>
        /// Gets or sets a value indicating the position in the file stream from where the
        /// transfer should start.
        /// </summary>
        public long StartPosition
        {
            get;
            set;
        }
    }
}
