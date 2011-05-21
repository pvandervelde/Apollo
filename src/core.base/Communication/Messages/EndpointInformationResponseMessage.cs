//-----------------------------------------------------------------------
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
    internal sealed class EndpointInformationResponseMessage : CommunicationMessage
    {
        private readonly List<ISerializedType> m_AvailableCommands =
            new List<ISerializedType>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointInformationResponseMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <param name="inResponseTo">
        ///     The ID number of the message to which the current message is a response.
        /// </param>
        /// <param name="availableCommands">
        ///     The array that contains the type information for all available <see cref="ICommandSet"/> interfaces.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="inResponseTo"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="availableCommands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="availableCommands"/> has no non-null elements.
        /// </exception>
        public EndpointInformationResponseMessage(EndpointId origin, MessageId inResponseTo, params Type[] availableCommands)
            : base(origin, inResponseTo)
        {
            {
                Enforce.Argument(() => availableCommands);
                Enforce.With<ArgumentException>(
                    (availableCommands.Length > 0) && (availableCommands[0] != null), 
                    Resources.Exceptions_Messages_ThereShouldBeAtLeastOneCommandToSend);
            }

            foreach (var type in availableCommands)
            {
                if (type != null)
                {
                    m_AvailableCommands.Add(CommandSetProxyExtensions.FromType(type));
                }
            }
        }

        /// <summary>
        /// Gets the collection of available commands.
        /// </summary>
        public IList<ISerializedType> Commands
        {
            get
            {
                return m_AvailableCommands;
            }
        }
    }
}
