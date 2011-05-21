//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Properties;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages
{
    /// <summary>
    /// Defines a message that indicates that the sending endpoint has connected to
    /// the current endpoint.
    /// </summary>
    [Serializable]
    internal sealed class EndpointConnectMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointConnectMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <param name="originatingAddress">
        /// The address of the originating endpoint.
        /// </param>
        /// <param name="channelType">
        ///     The <see cref="IChannelType"/> of the channel that was used to send this message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="originatingAddress"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="originatingAddress"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="channelType"/> does not implement the <see cref="IChannelType"/> interface.
        /// </exception>
        public EndpointConnectMessage(EndpointId origin, string originatingAddress, Type channelType)
            : base(origin)
        {
            {
                Enforce.Argument(() => originatingAddress);
                Enforce.With<ArgumentException>(
                    !string.IsNullOrWhiteSpace(originatingAddress), 
                    Resources.Exceptions_Messages_ChannelAddresssMustBeDefined);

                Enforce.Argument(() => channelType);
                Enforce.With<ArgumentException>(
                    typeof(IChannelType).IsAssignableFrom(channelType), 
                    Resources.Exceptions_Messages_AChannelTypeMustDeriveFromIChannelType);
            }

            Address = originatingAddress;
            ChannelType = channelType.FullName;
        }

        /// <summary>
        /// Gets a value indicating the URI of the channel.
        /// </summary>
        public string Address
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating what kind of channel was used
        /// to send this message.
        /// </summary>
        public string ChannelType
        {
            get;
            private set;
        }
    }
}
