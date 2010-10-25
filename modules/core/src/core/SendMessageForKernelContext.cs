//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Messaging;
using Apollo.Utils.Commands;
using Lokad;

namespace Apollo.Core
{
    /// <summary>
    /// Defines and <see cref="ICommandContext"/> for the <see cref="SendMessageForKernelCommand"/>.
    /// </summary>
    internal sealed class SendMessageForKernelContext : ICommandContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendMessageForKernelContext"/> class.
        /// </summary>
        /// <param name="recipient">The <see cref="DnsName"/> of the recipient.</param>
        /// <param name="messageToSend">The message that needs to be send.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="recipient"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="messageToSend"/> is <see langword="null" />.
        /// </exception>
        public SendMessageForKernelContext(DnsName recipient, MessageBody messageToSend)
        {
            {
                Enforce.Argument(() => recipient);
                Enforce.Argument(() => messageToSend);
            }

            Recipient = recipient;
            Message = messageToSend;
        }

        /// <summary>
        /// Gets the name of the recipient.
        /// </summary>
        /// <value>The recipient.</value>
        public DnsName Recipient
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public MessageBody Message
        {
            get;
            private set;
        }
    }
}
