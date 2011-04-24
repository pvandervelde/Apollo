//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        bool IsSignedOn
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
        void SignOn();

        /// <summary>
        /// Broadcasts a sign off message and disconnects from the network.
        /// </summary>
        void SignOff();

        /// <summary>
        /// An event raised when an endpoint has joined the network.
        /// </summary>
        event EventHandler<ConnectionInformationEventArgs> OnEndpointSignedOn;

        /// <summary>
        /// An event raised when an endpoint has left the network.
        /// </summary>
        event EventHandler<EndpointEventArgs> OnEndpointSignedOff;

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
        Task<ICommunicationMessage> SendMessageAndWaitForRespone(EndpointId endpoint, ICommunicationMessage message);

        /// <summary>
        /// Disconnects from the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        void DisconnectFromEndpoint(EndpointId endpoint);
    }
}
