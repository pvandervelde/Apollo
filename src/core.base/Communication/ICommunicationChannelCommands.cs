//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Net.NetworkInformation;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the commands for interaction with the actual communication channel.
    /// </summary>
    public interface ICommunicationChannelCommands : ICommandSet
    {
        /// <summary>
        /// Reconnects with the given endpoint.
        /// </summary>
        /// <returns>
        /// An observable containing the new ID number of the endpoint.
        /// </returns>
        IObservable<EndpointId> Reconnect();

        /// <summary>
        /// Pings an endpoint to establish that it is reachable and able to respond to messages.
        /// </summary>
        /// <returns>
        /// An observable containing the ping result.
        /// </returns>
        IObservable<PingReply> Ping();

        /// <summary>
        /// Transfers the given stream across the network to the given endpoint.
        /// </summary>
        /// <param name="size">The size of the data stream.</param>
        /// <param name="data">The data that should be transfered.</param>
        /// <returns>
        /// An observable indicating the success or failure of the operation.
        /// </returns>
        IObservable<object> Transfer(long size, Stream data);
    }
}
