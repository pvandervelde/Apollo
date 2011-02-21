﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the interface for objects that handle communication with a remote application.
    /// </summary>
    internal interface ICommunicationChannel
    {
        /// <summary>
        /// Gets a value indicating the ID number of the channel.
        /// </summary>
        EndpointId Id
        {
            get;
        }

        /// <summary>
        /// Opens the channel and provides information on how to connect to the given channel.
        /// </summary>
        /// <returns>
        /// The information describing how to connect to the current channel.
        /// </returns>
        ChannelConnectionInformation OpenChannel();

        /// <summary>
        /// Closes the current channel.
        /// </summary>
        void Close();

        /// <summary>
        /// Connects to a channel by using the information provided in the connection information
        /// object.
        /// </summary>
        /// <param name="connection">The information necessary to connect to the remote channel.</param>
        void ConnectTo(ChannelConnectionInformation connection);

        /// <summary>
        /// Disconnects from the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint from which the channel needs to disconnect.</param>
        void DisconnectFrom(EndpointId endpoint);

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
        /// <param name="file">The file stream that contains the file that should be transferred.</param>
        /// <param name="transferInformation">
        /// The information which describes the data to be transferred and the remote connection over
        /// which the data is transferred.
        /// </param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <returns>
        /// An task that indicates when the transfer is complete.
        /// </returns>
        Task TransferData(FileStream file, StreamTransferInformation transferInformation, CancellationToken token);

        /// <summary>
        /// Sends the given message to the receiving endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message should be send.</param>
        /// <param name="message">The message that should be send.</param>
        void Send(EndpointId endpoint, ICommunicationMessage message);

        /// <summary>
        /// An event raised when a new message is received.
        /// </summary>
        event EventHandler<MessageEventArgs> OnReceive;

        /// <summary>
        /// An event raised when the other side of the connection is closed.
        /// </summary>
        event EventHandler<ChannelClosedEventArgs> OnClosed;
    }
}
