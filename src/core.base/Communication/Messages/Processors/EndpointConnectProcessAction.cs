//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apollo.Core.Base.Properties;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an <see cref="EndpointConnectMessage"/>.
    /// </summary>
    internal sealed class EndpointConnectProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// The collection that contains all the known <see cref="IChannelType"/> of the 
        /// communication channel from which the messages that are being processed originate.
        /// </summary>
        private readonly List<Type> m_ConnectedChannelTypes = new List<Type>();

        /// <summary>
        /// The object that processes connection information.
        /// </summary>
        private readonly IAcceptExternalEndpointInformation m_ConnectionInformationSink;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointConnectProcessAction"/> class.
        /// </summary>
        /// <param name="connectionInformationSink">
        /// The object that forwards the information about the newly connected endpoint.
        /// </param>
        /// <param name="channelTypes">
        /// The collection that contains all possible <see cref="IChannelType"/> types for the 
        /// communication channel from which the messages that are being processed originate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="connectionInformationSink"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelTypes"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if one of the entries in <paramref name="channelTypes"/> does not implement the <see cref="IChannelType"/> interface.
        /// </exception>
        public EndpointConnectProcessAction(IAcceptExternalEndpointInformation connectionInformationSink, IEnumerable<Type> channelTypes)
        {
            {
                Enforce.Argument(() => connectionInformationSink);

                Enforce.Argument(() => channelTypes);
                Enforce.With<ArgumentException>(channelTypes.All(t => typeof(IChannelType).IsAssignableFrom(t)), Resources.Exceptions_Messages_AChannelTypeMustDeriveFromIChannelType);
            }

            m_ConnectedChannelTypes.AddRange(channelTypes);
            m_ConnectionInformationSink = connectionInformationSink;
        }

        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        public Type MessageTypeToProcess
        {
            get
            {
                return typeof(EndpointConnectMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as EndpointConnectMessage;
            Debug.Assert(msg != null, "The message is of the incorrect type.");

            Type channelType = m_ConnectedChannelTypes.Find(t => string.Equals(t.FullName, msg.ChannelType, StringComparison.Ordinal));
            m_ConnectionInformationSink.RecentlyConnectedEndpoint(msg.OriginatingEndpoint, channelType, new Uri(msg.Address));
        }
    }
}
