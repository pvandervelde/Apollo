//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;

namespace Test.Manual.Console.Views
{
    /// <summary>
    /// Forwards communication commands to the communication layer.
    /// </summary>
    internal sealed class CommunicationPassThrough : IHandleCommunication
    {
        /// <summary>
        /// The communication layer which does the actual communication work.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationPassThrough"/> class.
        /// </summary>
        /// <param name="layer">The communication layer which does the actual communication work.</param>
        public CommunicationPassThrough(ICommunicationLayer layer)
        {
            m_Layer = layer;
        }

        /// <summary>
        /// Gets a value indicating whether the communication layer has been
        /// activated.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return m_Layer.IsSignedOn;
            }
        }

        /// <summary>
        /// Returns a value indicating if connection information is available for
        /// the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>
        /// <see langword="true" /> if connection information is available for the given endpoint;
        /// otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsEndpointKnown(EndpointId endpoint)
        {
            return m_Layer.IsEndpointContactable(endpoint);
        }

        /// <summary>
        /// Connects to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        public void ConnectTo(EndpointId endpoint)
        {
            m_Layer.ConnectToEndpoint(endpoint);
        }

        /// <summary>
        /// Disconnects from the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        public void DisconnectFrom(EndpointId endpoint)
        {
            m_Layer.DisconnectFromEndpoint(endpoint);
        }

        /// <summary>
        /// Sends an echo message to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message should be send.</param>
        /// <param name="messageText">The text of the message.</param>
        public void SendEchoMessageTo(EndpointId endpoint, string messageText)
        {
            m_Layer.SendMessageTo(endpoint, new EchoMessage(m_Layer.Id, messageText));
        }

        /// <summary>
        /// Closes the connections to all endpoints.
        /// </summary>
        public void Close()
        {
            m_Layer.SignOff();
        }
    }
}
