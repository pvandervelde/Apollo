//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Base.Communication;
using Autofac;
using Test.Manual.Console.Models;

namespace Test.Manual.Console
{
    /// <summary>
    /// Provides the methods necessary for handling incoming messages.
    /// </summary>
    internal sealed class ApplicationCentral : IFormTheApplicationCenter, IStartable
    {
        /// <summary>
        /// The layer from which the incoming messages are received.
        /// </summary>
        private readonly ICommunicationLayer m_CommunicationLayer;

        /// <summary>
        /// The object that stores information about the current connections and the messages 
        /// that have been received.
        /// </summary>
        private ConnectionViewModel m_ConnectionStateInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCentral"/> class.
        /// </summary>
        /// <param name="communicationLayer">The layer from which incoming messages are recieved.</param>
        /// <param name="connectionStateInformation">The object that stores information about the current connections and received messages.</param>
        public ApplicationCentral(ICommunicationLayer communicationLayer, ConnectionViewModel connectionStateInformation)
        {
            m_CommunicationLayer = communicationLayer;
            m_CommunicationLayer.OnEndpointSignedIn += (s, e) => OnEndpointSignedOn(e.ConnectionInformation);
            m_CommunicationLayer.OnEndpointSignedOut += (s, e) => OnEndpointSignedOff(e.Endpoint);

            m_ConnectionStateInformation = connectionStateInformation;
        }

        private void OnEndpointSignedOn(ChannelConnectionInformation connectionInfo)
        {
            m_ConnectionStateInformation.AddKnownEndpoint(
                new ConnectionInformationViewModel(
                    connectionInfo.Id, 
                    connectionInfo.Address.AbsoluteUri, 
                    connectionInfo.ChannelType.ToString()));
        }

        private void OnEndpointSignedOff(EndpointId endpointId)
        {
            m_ConnectionStateInformation.RemoveKnownEndpoint(endpointId);
        }

        /// <summary>
        /// Perform once-off startup processing.
        /// </summary>
        public void Start()
        {
            m_CommunicationLayer.SignIn();

            var localEndpoints = m_CommunicationLayer.LocalConnectionPoints();
            foreach (var endpoint in localEndpoints)
            {
                m_ConnectionStateInformation.AddLocalEndpoint(
                    new ConnectionInformationViewModel(
                        endpoint.Id,
                        endpoint.Address.AbsoluteUri,
                        endpoint.ChannelType.ToString()));
            }
        }
    }
}
