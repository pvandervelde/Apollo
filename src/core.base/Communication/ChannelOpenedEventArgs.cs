//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Properties;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines an <see cref="EventArgs"/> class that indicates which channel has been opened.
    /// </summary>
    public sealed class ChannelOpenedEventArgs : EventArgs
    {
        /// <summary>
        /// The ID of the endpoint for which a channel was opened.
        /// </summary>
        private readonly EndpointId m_Endpoint;

        /// <summary>
        /// The type of channel that was opened.
        /// </summary>
        private readonly Type m_ChannelType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelOpenedEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint for which the channel was opened.</param>
        /// <param name="channelType">The type of channel which was opened.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="channelType"/> does not inherit from <see cref="IChannelType"/>.
        /// </exception>
        public ChannelOpenedEventArgs(EndpointId endpoint, Type channelType)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);

                Lokad.Enforce.Argument(() => channelType);
                Lokad.Enforce.With<ArgumentException>(
                    typeof(IChannelType).IsAssignableFrom(channelType),
                    Resources.Exceptions_Messages_AChannelTypeMustDeriveFromIChannelType);
            }

            m_Endpoint = endpoint;
            m_ChannelType = channelType;
        }

        /// <summary>
        /// Gets the ID of the endpoint for which a channel was opened.
        /// </summary>
        public EndpointId Endpoint
        {
            get
            {
                return m_Endpoint;
            }
        }

        /// <summary>
        /// Gets the type of channel that was opened.
        /// </summary>
        public Type ChannelType
        {
            get
            {
                return m_ChannelType;
            }
        }
    }
}
