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
    /// Defines a message that contains information about a newly registered command.
    /// </summary>
    [Serializable]
    internal sealed class NewCommandRegisteredMessage : CommunicationMessage
    {
        /// <summary>
        /// The newly registered command.
        /// </summary>
        private readonly ISerializedType m_Command;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewCommandRegisteredMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <param name="commandType">
        ///     The newly available <see cref="ICommandSet"/> interface.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandType"/> is <see langword="null" />.
        /// </exception>
        public NewCommandRegisteredMessage(EndpointId origin, Type commandType)
            : base(origin)
        {
            {
                Enforce.Argument(() => commandType);
            }

            m_Command = ProxyExtensions.FromType(commandType);
        }

        /// <summary>
        /// Gets the newly registered command.
        /// </summary>
        public ISerializedType Command
        {
            get
            {
                return m_Command;
            }
        }
    }
}
