//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Properties;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Stores the information required to contact a given WCF endpoint on a specific machine.
    /// </summary>
    [Serializable]
    internal sealed class ChannelConnectionInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelConnectionInformation"/> class.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint for which this connection information is valid.</param>
        /// <param name="channelType">
        ///     The type of the <see cref="IChannelType"/> which indicates which kind of channel this connection
        ///     information describes.
        /// </param>
        /// <param name="address">The full URI for the channel.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="channelType"/> does not inherit from <see cref="IChannelType"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="address"/> is <see langword="null" />.
        /// </exception>
        public ChannelConnectionInformation(EndpointId endpoint, Type channelType, Uri address)
        {
            {
                Enforce.Argument(() => endpoint);

                Enforce.Argument(() => channelType);
                Enforce.With<ArgumentException>(typeof(IChannelType).IsAssignableFrom(channelType), Resources.Exceptions_Messages_AChannelTypeMustDeriveFromIChannelType);

                Enforce.Argument(() => address);
            }

            Id = endpoint;
            ChannelType = channelType;
            Address = address;
        }

        /// <summary>
        /// Gets a value indicating the ID of the endpoint for which this information
        /// is valid.
        /// </summary>
        public EndpointId Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating which type the channel is. The <see cref="Type"/>
        /// will be a derivative of <see cref="IChannelType"/>.
        /// </summary>
        public Type ChannelType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the URI of the channel.
        /// </summary>
        public Uri Address
        {
            get;
            private set;
        }
    }
}
