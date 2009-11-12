//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Apollo.Core.Messages;
using Lokad;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the base class for messages send inside the core of the Apollo system.
    /// </summary>
    [Serializable]
    public sealed class KernelMessage : IEquatable<KernelMessage>
    {
        /// <summary>
        /// The header of the the current message.
        /// </summary>
        private readonly MessageHeader m_Header;

        /// <summary>
        /// The data attached to this message.
        /// </summary>
        private readonly MessageBody m_Body;

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelMessage"/> class.
        /// </summary>
        /// <param name="messageToCopy">The message to copy.</param>
        public KernelMessage(KernelMessage messageToCopy)
        {
            {
                Enforce.Argument(() => messageToCopy);
            }

            m_Header = new MessageHeader(messageToCopy.Header);
            m_Body = messageToCopy.Body.Copy();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelMessage"/> class.
        /// </summary>
        /// <param name="header">The header which identifies the message.</param>
        /// <param name="body">The data for the message.</param>
        public KernelMessage(MessageHeader header, MessageBody body)
        {
            {
                Enforce.Argument(() => body);
            }

            m_Header = header;
            m_Body = body;
        }

        /// <summary>
        /// Gets the header which identifies this message.
        /// </summary>
        /// <value>The header.</value>
        public MessageHeader Header
        {
            get
            {
                return m_Header;
            }
        }

        /// <summary>
        /// Gets the data for the message.
        /// </summary>
        /// <value>The data.</value>
        public MessageBody Body
        {
            get
            {
                return m_Body;
            }
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(KernelMessage other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other != null)
            {
                return other.Header.Equals(m_Header);
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            KernelMessage other = obj as KernelMessage;
            if (other != null)
            {
                return Equals(other);
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
            return m_Header.GetHashCode();
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
            return string.Format(CultureInfo.InvariantCulture, 
                @"Message {0} send by {1} to {2} in reply to {3} with information {4}", 
                m_Header.Id, m_Header.Sender, m_Header.Recipient, m_Header.InReplyTo, m_Body);
        }
    }
}
