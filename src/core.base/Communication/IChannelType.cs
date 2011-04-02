//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the interface for objects that provide information about a specific type of
    /// WCF channel, e.g. TCP.
    /// </summary>
    internal interface IChannelType
    {
        /// <summary>
        /// Generates a new URI for the channel.
        /// </summary>
        /// <returns>
        /// The newly generated URI.
        /// </returns>
        Uri GenerateNewChannelUri();

        /// <summary>
        /// Generates a new address for the channel endpoint.
        /// </summary>
        /// <returns>
        /// The newly generated address for the channel endpoint.
        /// </returns>
        string GenerateNewAddress();

        /// <summary>
        /// Generates a new binding object for the channel.
        /// </summary>
        /// <returns>
        /// The newly generated binding.
        /// </returns>
        Binding GenerateBinding();

        /// <summary>
        /// Creates the required channel(s) to receive a data stream across the network and returns
        /// the connection information and the task responsible for handling the data reception.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="localFile"/> does not exist a new file will be created with the given path. If
        /// it does exist then the data will be appended to it.
        /// </remarks>
        /// <param name="localFile">The full file path to which the network stream should be written.</param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <returns>
        /// The connection information necessary to connect to the newly created channel and the task 
        /// responsible for handling the data reception.
        /// </returns>
        Tuple<StreamTransferInformation, Task<FileInfo>> PrepareForDataReception(string localFile, CancellationToken token);

        /// <summary>
        /// Transfers the data to the receiving endpoint.
        /// </summary>
        /// <param name="filePath">The file path to the file that should be transferred.</param>
        /// <param name="transferInformation">
        /// The information which describes the data to be transferred and the remote connection over
        /// which the data is transferred.
        /// </param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <returns>
        /// An task that indicates when the transfer is complete.
        /// </returns>
        Task TransferData(string filePath, StreamTransferInformation transferInformation, CancellationToken token);
    }
}
