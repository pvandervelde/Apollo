//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Properties;
using Lokad;

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// Stores the information required to succesfully send a message
    /// from one service to another service.
    /// </summary>
    [Serializable]
    public sealed class MessageHeader : IEquatable<MessageHeader>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(MessageHeader first, MessageHeader second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(MessageHeader first, MessageHeader second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return !nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeader"/> class and
        /// sets it up to send a message to the specified recipient.
        /// </summary>
        /// <param name="messageId">The unique ID number of the message.</param>
        /// <param name="senderDns">The sender of the message.</param>
        /// <param name="recipientDns">The recipient of the message.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="messageId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="messageId"/> is equal to the <see cref="MessageId.None"/> ID.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="senderDns"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="senderDns"/> is equal to the <see cref="DnsName.Nobody"/> name.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="recipientDns"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="recipientDns"/> is equal to the <see cref="DnsName.Nobody"/> name.
        /// </exception>
        public MessageHeader(MessageId messageId, DnsName senderDns, DnsName recipientDns)
            : this(messageId, senderDns, recipientDns, MessageId.None)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeader"/> class.
        /// </summary>
        /// <param name="messageId">The unique ID number of the message.</param>
        /// <param name="senderDns">The sender of the message.</param>
        /// <param name="recipientDns">The recipient of the message.</param>
        /// <param name="replyToId">
        ///     The ID number of the message to which the current message is a reply.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="messageId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="messageId"/> is equal to the <see cref="MessageId.None"/> ID.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="senderDns"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="senderDns"/> is equal to the <see cref="DnsName.Nobody"/> name.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="recipientDns"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="recipientDns"/> is equal to the <see cref="DnsName.Nobody"/> name.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="replyToId"/> is <see langword="null" />.
        /// </exception>
        public MessageHeader(MessageId messageId, DnsName senderDns, DnsName recipientDns, MessageId replyToId)
        {
            {
                Enforce.Argument(() => messageId);
                Enforce.With<ArgumentException>(!messageId.Equals(MessageId.None), Resources_NonTranslatable.Exception_Messages_CannotCreateAMessageWithoutId);

                Enforce.Argument(() => senderDns);
                Enforce.With<ArgumentException>(!senderDns.Equals(DnsName.Nobody), Resources_NonTranslatable.Exception_Messages_CannotCreateAMessageWithoutSender);

                Enforce.Argument(() => recipientDns);
                Enforce.With<ArgumentException>(!recipientDns.Equals(DnsName.Nobody), Resources_NonTranslatable.Exception_Messages_CannotSendAMessageToNoService);
                Enforce.With<ArgumentException>(!recipientDns.Equals(senderDns), Resources_NonTranslatable.Exception_Messages_CannotSendAMessageBackToTheSender_WithDnsName, senderDns);

                Enforce.Argument(() => replyToId);
            }

            Id = messageId;
            Recipient = recipientDns;
            Sender = senderDns;
            InReplyTo = replyToId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeader"/> class by
        /// copying the provided <c>MessageHeader</c>.
        /// </summary>
        /// <param name="headerToCopy">The message header which should be copied.</param>
        public MessageHeader(MessageHeader headerToCopy) 
            : this(headerToCopy.Id, headerToCopy.Sender, headerToCopy.Recipient, headerToCopy.InReplyTo)
        { 
        }

        /// <summary>
        /// Gets the sender of this message.
        /// </summary>
        /// <value>The sender.</value>
        public DnsName Sender
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the recipient for this message.
        /// </summary>
        /// <value>The recipient.</value>
        public DnsName Recipient
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ID of this message.
        /// </summary>
        /// <value>The ID number of this message.</value>
        public MessageId Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ID number of the message to which this message is a reply.
        /// </summary>
        /// <value>The ID number of the message.</value>
        public MessageId InReplyTo
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether the specified <c>MessageHeader</c> is equal to this instance.
        /// </summary>
        /// <param name="other">The <c>MessageHeader</c> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <c>MessageHeader</c> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(MessageHeader other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return other.Id.Equals(Id);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var header = obj as MessageHeader;
            if (header != null)
            {
                return Equals(header);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            // No need to translate this. This should never show up in the 
            // user interface.
            return string.Format(CultureInfo.InvariantCulture, @"Message [{0}] send by [{1}] to [{2}] in reply to [{3}]", Id, Sender, Recipient, InReplyTo);
        }
    }
}