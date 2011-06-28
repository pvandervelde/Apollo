//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages
{
    /// <summary>
    /// Defines a message that requests the download of a specific file from the receiver.
    /// </summary>
    [Serializable]
    internal sealed class DataDownloadRequestMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataDownloadRequestMessage"/> class.
        /// </summary>
        /// <param name="origin">The ID of the endpoint that send the message.</param>
        /// <param name="token">The token that indicates which file should be uploaded.</param>
        /// <param name="streamToTransfer">Describes the stream that should be transferred.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="token"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="streamToTransfer"/> is <see langword="null" />.
        /// </exception>
        public DataDownloadRequestMessage(EndpointId origin, UploadToken token, StreamTransferInformation streamToTransfer)
            : base(origin)
        {
            {
                Enforce.Argument(() => token);
                Enforce.Argument(() => streamToTransfer);
            }

            Token = token;
            TransferInformation = streamToTransfer;
        }

        /// <summary>
        /// Gets the token that indicates which file should be uploaded.
        /// </summary>
        public UploadToken Token
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the information describing what stream should be transferred and how
        /// this stream should be transferred.
        /// </summary>
        public StreamTransferInformation TransferInformation
        {
            get;
            private set;
        }
    }
}
