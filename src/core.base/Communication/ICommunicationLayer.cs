//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods for communicating with a remote endpoint.
    /// </summary>
    internal interface ICommunicationLayer
    {
        /// <summary>
        /// An event raised when the availability of an endpoint changes.
        /// </summary>
        event EventHandler<EndpointAvailabilityEventArgs> OnAvailabilityChange;

        /// <summary>
        /// An event raised when an endpoint goes offline permanently.
        /// </summary>
        event EventHandler<EndpointEventArgs> OnTerminate;

        /// <summary>
        /// Returns a value indicating if the given endpoint is available on the network.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if the endpoint can be reached over the network. Otherwise; <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsEndpointAvailable(EndpointId endpoint);

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
    }
}
