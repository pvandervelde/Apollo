//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Messages
{
    /// <summary>
    /// Defines an ID number for messages.
    /// </summary>
    [Serializable]
    public sealed class MessageId : IIsId, IEquatable<MessageId>
    {
        private readonly static MessageId s_NoneId = new MessageId(Guid.NewGuid());

        /// <summary>
        /// Returns the ID number for messages without an ID.
        /// </summary>
        /// <value>The none ID.</value>
        public static MessageId None
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private readonly Guid m_Id;

        private MessageId(Guid id)
        {
            m_Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageId"/> class.
        /// </summary>
        /// <param name="idToCopy">The id to copy.</param>
        public MessageId(MessageId idToCopy)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageId"/> class.
        /// </summary>
        /// <param name="idToCopy">The id to copy.</param>
        public MessageId(IIsId idToCopy)
        { }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns></returns>
        public MessageId Copy()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(MessageId other)
        {
            throw new NotImplementedException();
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
            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}