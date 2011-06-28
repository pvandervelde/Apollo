//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods for storing information about the way a stream should 
    /// be transferred over a named pipe.
    /// </summary>
    [Serializable]
    internal sealed class NamedPipeStreamTransferInformation : StreamTransferInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeStreamTransferInformation"/> class.
        /// </summary>
        public NamedPipeStreamTransferInformation()
            : base(typeof(NamedPipeChannelType))
        { 
        }

        /// <summary>
        /// Gets or sets a value indicating the name of the pipe.
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }
}
