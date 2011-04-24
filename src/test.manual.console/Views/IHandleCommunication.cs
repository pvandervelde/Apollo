//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;

namespace Test.Manual.Console.Views
{
    /// <summary>
    /// Defines the interface for objects that transmit communication commands 
    /// to the communication layer.
    /// </summary>
    internal interface IHandleCommunication
    {
        /// <summary>
        /// Gets a value indicating whether the communication layer has been
        /// activated.
        /// </summary>
        bool IsConnected 
        { 
            get;
        }

        /// <summary>
        /// Returns a value indicating if connection information is available for
        /// the given endpoint.
        /// </summary>
        /// <param name="endpointId">The endpoint.</param>
        /// <returns>
        /// <see langword="true" /> if connection information is available for the given endpoint;
        /// otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsEndpointKnown(EndpointId endpointId);

        /// <summary>
        /// Connects to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        void ConnectTo(EndpointId endpoint);

        /// <summary>
        /// Disconnects from the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        void DisconnectFrom(EndpointId endpoint);

        /// <summary>
        /// Sends an echo message to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message should be send.</param>
        /// <param name="messageText">The text of the message.</param>
        void SendEchoMessageTo(EndpointId endpoint, string messageText);

        /// <summary>
        /// Closes the connections to all endpoints.
        /// </summary>
        void Close();
    }
}
