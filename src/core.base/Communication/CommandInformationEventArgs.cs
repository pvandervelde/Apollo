//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Stores <see cref="EventArgs"/> describing the registration of a new command on a
    /// remote endpoint.
    /// </summary>
    internal sealed class CommandInformationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInformationEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint on which the command was registered.</param>
        /// <param name="command">The command that was registered.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="command"/> is <see langword="null" />.
        /// </exception>
        public CommandInformationEventArgs(EndpointId endpoint, ISerializedType command)
        {
            {
                Enforce.Argument(() => endpoint);
                Enforce.Argument(() => command);
            }

            Endpoint = endpoint;
            Command = command;
        }

        /// <summary>
        /// Gets the ID number of the remote endpoint.
        /// </summary>
        public EndpointId Endpoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the command that was registered on the remote endpoint.
        /// </summary>
        public ISerializedType Command
        {
            get;
            private set;
        }
    }
}
