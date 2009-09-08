//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Lokad;

namespace Apollo.Core.Messages
{
    /// <summary>
    /// Stores the information required to succesfully send a message
    /// from one service to another service.
    /// </summary>
    [Serializable]
    public sealed class MessageHeader : IEquatable<MessageHeader>
    {
        /// <summary>
        /// The unique ID number of this message.
        /// </summary>
        private readonly MessageId m_Id = MessageId.None;

        /// <summary>
        /// The ID number of the message to which this message is a reply.
        /// </summary>
        private readonly MessageId m_InReplyTo = MessageId.None;

        /// <summary>
        /// The unique name of the service that send this message.
        /// </summary>
        private readonly DnsName m_Sender = DnsName.Nobody;

        /// <summary>
        /// The unique name of the service to which this message is send.
        /// </summary>
        private readonly DnsName m_Recipient = DnsName.Nobody;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeader"/> struct and 
        /// sets it up to send a message to all services, excluding the sending
        /// service.
        /// </summary>
        /// <param name="id">The unique ID number of the message.</param>
        /// <param name="sender">The sender of the message.</param>
        public MessageHeader(MessageId id, DnsName sender)
            : this(id, sender, DnsName.AllServices, MessageId.None)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeader"/> struct and
        /// sets it up to send a message to the specified recipient.
        /// </summary>
        /// <param name="id">The unique ID number of the message.</param>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="recipient">The recipient of the message.</param>
        public MessageHeader(MessageId id, DnsName sender, DnsName recipient)
            : this(id, sender, recipient, MessageId.None)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeader"/> struct.
        /// </summary>
        /// <param name="id">The unique ID number of the message.</param>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="recipient">The recipient of the message.</param>
        /// <param name="inReplyTo">
        ///     The ID number of the message to which the current message is a reply.
        /// </param>
        public MessageHeader(MessageId id, DnsName sender, DnsName recipient, MessageId inReplyTo)
        {
            {
                Enforce.That(!id.Equals(MessageId.None));

                Enforce.That(!sender.Equals(DnsName.Nobody));
                Enforce.That(!sender.Equals(DnsName.AllServices));
            }

            m_Recipient = recipient;
            m_Sender = sender;
            m_InReplyTo = inReplyTo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeader"/> struct by
        /// copying the provided <c>MessageHeader</c>.
        /// </summary>
        /// <param name="headerToCopy">The message header which should be copied.</param>
        public MessageHeader(MessageHeader headerToCopy) 
            : this(headerToCopy.Id, headerToCopy.Sender, headerToCopy.Recipient, headerToCopy.InReplyTo)
        { }

        /// <summary>
        /// Gets the sender of this message.
        /// </summary>
        /// <value>The sender.</value>
        public DnsName Sender
        {
            get
            {
                return m_Sender;
            }
        }

        /// <summary>
        /// Gets the recipient for this message.
        /// </summary>
        /// <value>The recipient.</value>
        public DnsName Recipient
        {
            get 
            {
                return m_Recipient;
            }
        }

        /// <summary>
        /// Gets the ID of this message.
        /// </summary>
        /// <value>The ID.</value>
        public MessageId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Returns the ID number of the message to which this message is a reply.
        /// </summary>
        /// <value>The ID number of the message.</value>
        public MessageId InReplyTo
        {
            get
            {
                return m_InReplyTo;
            }
        }

        /// <summary>
        /// Determines whether the specified <c>MessageHeader</c> is equal to this instance.
        /// </summary>
        /// <param name="other">The <c>MessageHeader</c> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <c>MessageHeader</c> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(MessageHeader other)
        {
            return other.Id.Equals(Id);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is MessageHeader)
            {
                return Equals((MessageHeader)obj);
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
            return m_Id.GetHashCode();
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
            return string.Format(CultureInfo.InvariantCulture, @"Message {0} send by {1} to {2} in reply to {3}", m_Id, m_Sender, m_Recipient, m_InReplyTo);
        }
    }
}