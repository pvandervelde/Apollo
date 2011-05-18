//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages
{
    /// <summary>
    /// Defines a message that indicates that the sending endpoint is about to disconnnect
    /// from the network.
    /// </summary>
    [Serializable]
    internal sealed class EndpointDisconnectMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointDisconnectMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        public EndpointDisconnectMessage(EndpointId origin)
            : this(origin, string.Empty)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointDisconnectMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <param name="closingReason">
        /// The reason the channel is about to be closed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="closingReason"/> is <see langword="null" />.
        /// </exception>
        public EndpointDisconnectMessage(EndpointId origin, string closingReason)
            : base(origin)
        {
            {
                Enforce.Argument(() => closingReason);
            }

            ClosingReason = closingReason;
        }

        /// <summary>
        /// Gets a value indicating why the channel is being closed.
        /// </summary>
        public string ClosingReason
        {
            get;
            private set;
        }
    }
}
