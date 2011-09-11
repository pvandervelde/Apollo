﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Base.Properties;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages
{
    /// <summary>
    /// Defines a message that responds to an information request the capabilities
    /// of an endpoint.
    /// </summary>
    [Serializable]
    internal sealed class EndpointProxyTypesResponseMessage : CommunicationMessage
    {
        private readonly List<ISerializedType> m_AvailableCommands =
            new List<ISerializedType>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointProxyTypesResponseMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <param name="inResponseTo">
        ///     The ID number of the message to which the current message is a response.
        /// </param>
        /// <param name="availableProxies">
        ///     The array that contains the type information for all available <see cref="ICommandSet"/> interfaces.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="inResponseTo"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="availableProxies"/> is <see langword="null" />.
        /// </exception>
        public EndpointProxyTypesResponseMessage(EndpointId origin, MessageId inResponseTo, params Type[] availableProxies)
            : base(origin, inResponseTo)
        {
            {
                Enforce.Argument(() => availableProxies);
            }

            foreach (var type in availableProxies)
            {
                if (type != null)
                {
                    m_AvailableCommands.Add(ProxyExtensions.FromType(type));
                }
            }
        }

        /// <summary>
        /// Gets the collection of available commands.
        /// </summary>
        public IList<ISerializedType> ProxyTypes
        {
            get
            {
                return m_AvailableCommands;
            }
        }
    }
}
