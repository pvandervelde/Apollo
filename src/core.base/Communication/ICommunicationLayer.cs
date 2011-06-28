//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods for communicating with a remote endpoint.
    /// </summary>
    internal interface ICommunicationLayer
    {
        /// <summary>
        /// Gets the endpoint ID of the local endpoint.
        /// </summary>
        EndpointId Id
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the communication layer has signed on with
        /// the network.
        /// </summary>
        bool IsSignedIn
        { 
            get; 
        }

        /// <summary>
        /// Returns a collection containing information about the local connection points.
        /// </summary>
        /// <returns>
        /// The collection that describes the local connection points.
        /// </returns>
        IEnumerable<ChannelConnectionInformation> LocalConnectionPoints();

        /// <summary>
        /// Connects to the network and broadcasts a sign on message.
        /// </summary>
        void SignIn();

        /// <summary>
        /// Broadcasts a sign off message and disconnects from the network.
        /// </summary>
        void SignOut();

        /// <summary>
        /// An event raised when an endpoint has joined the network.
        /// </summary>
        event EventHandler<ConnectionInformationEventArgs> OnEndpointSignedIn;

        /// <summary>
        /// An event raised when an endpoint has left the network.
        /// </summary>
        event EventHandler<EndpointEventArgs> OnEndpointSignedOut;

        /// <summary>
        /// Returns a value indicating if the given endpoint has provided the information required to
        /// contact it if it isn't offline.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if the endpoint has provided the information necessary to contact 
        ///     it over the network. Otherwise; <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsEndpointContactable(EndpointId endpoint);

        /// <summary>
        /// Connects to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        void ConnectToEndpoint(EndpointId endpoint);

        /// <summary>
        /// Sends the given message to the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message has to be send.</param>
        /// <param name="message">The message that has to be send.</param>
        void SendMessageTo(EndpointId endpoint, ICommunicationMessage message);

        /// <summary>
        /// Sends the given message to the specified endpoint and returns a task that
        /// will eventually contain the return message.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message has to be send.</param>
        /// <param name="message">The message that has to be send.</param>
        /// <returns>A task object that will eventually contain the response message.</returns>
        Task<ICommunicationMessage> SendMessageAndWaitForResponse(EndpointId endpoint, ICommunicationMessage message);

        /// <summary>
        /// Uploads a given file to a specific endpoint.
        /// </summary>
        /// <param name="filePath">The full path to the file that should be transferred.</param>
        /// <param name="transferInfo">The object that provides the upload information.</param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <returns>
        ///     A task that will return once the upload is complete.
        /// </returns>
        Task UploadData(string filePath, StreamTransferInformation transferInfo, CancellationToken token);

        /// <summary>
        /// Downloads a given file from a specific endpoint.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="localFile"/> does not exist a new file will be created with the given path. If
        /// it does exist then the data will be appended to it.
        /// </remarks>
        /// <param name="endpointToDownloadFrom">The endpoint ID of the endpoint from which the data should be transferred.</param>
        /// <param name="uploadToken">The token that indicates which file should be uploaded.</param>
        /// <param name="localFile">The full file path to which the network stream should be written.</param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <returns>
        /// The task which will return the pointer to the file once the download is complete.
        /// </returns>
        Task<Stream> DownloadData(EndpointId endpointToDownloadFrom, UploadToken uploadToken, string localFile, CancellationToken token);

        /// <summary>
        /// Disconnects from the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        void DisconnectFromEndpoint(EndpointId endpoint);
    }
}
