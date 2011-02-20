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
    /// Defines an <see cref="EventArgs"/> class that indicates which channel has been closed.
    /// </summary>
    public sealed class ChannelClosedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelClosedEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint that was closed.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        public ChannelClosedEventArgs(EndpointId endpoint)
        {
            {
                Enforce.Argument(() => endpoint);
            }

            ClosedChannel = endpoint;
        }

        /// <summary>
        /// Gets a value indicating the ID of the closed channel.
        /// </summary>
        public EndpointId ClosedChannel
        {
            get;
            private set;
        }
    }
}
