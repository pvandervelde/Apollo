﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Net;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods for storing information about the way a stream should 
    /// be transferred over the network.
    /// </summary>
    internal sealed class StreamTransferInformation
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

        /// <summary>
        /// Gets or sets a value indicating the IP address of the remote computer to which 
        /// the file should be transferred.
        /// </summary>
        public IPAddress IPAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating the port on the remote computer on which the computer
        /// is listening for the file transfer.
        /// </summary>
        public int Port
        {
            get;
            set;
        }
    }
}
