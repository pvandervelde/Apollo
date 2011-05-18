//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the interface for sending messages through a WCF channel.
    /// </summary>
    internal interface ISendingEndpoint
    {
        /// <summary>
        /// Returns the collection of known endpoints.
        /// </summary>
        /// <returns>
        /// The collection of known endpoints.
        /// </returns>
        IEnumerable<EndpointId> KnownEndpoints();

        /// <summary>
        /// Sends the given message.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message should be send.</param>
        /// <param name="message">The message to be send.</param>
        void Send(EndpointId endpoint, ICommunicationMessage message);

        /// <summary>
        /// Closes the channel that connects to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint to which the connection should be closed.</param>
        void CloseChannelTo(EndpointId endpoint);
    }
}
