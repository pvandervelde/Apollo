//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils;

namespace Apollo.Core.Messages
{
    /// <summary>
    /// Defines an ID number for messages.
    /// </summary>
    [Serializable]
    public sealed class MessageId : Id<MessageId, Guid>
    {
        /// <summary>
        /// Defines the ID number for a message without an ID number.
        /// </summary>
        /// <design>
        /// The GUID is hard-coded in order to keep it constant between different
        /// revisions of the software.
        /// </design>
        private static readonly Guid s_NoneId = new Guid(@"6C01758E-D1E7-4DF8-B847-51ED22083E96");

        /// <summary>
        /// Gets the ID number for messages without an ID.
        /// </summary>
        /// <value>The none ID.</value>
        public static MessageId None
        {
            get
            {
                return new MessageId(s_NoneId);
            }
        }

        /// <summary>
        /// Generates the next <see cref="MessageId"/> in the sequence.
        /// </summary>
        /// <returns>
        /// A new and unique message ID number.
        /// </returns>
        public static MessageId Next()
        {
            return new MessageId(Guid.NewGuid());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageId"/> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> which serves as the internal ID number.</param>
        private MessageId(Guid id)
            : base(id)
        {
        }

        /// <summary>
        /// Clones the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A new <see cref="MessageId"/> which is an exact copy of the original.
        /// </returns>
        protected override MessageId Clone(Guid value)
        {
            return new MessageId(value);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Message ID with number: {0}", InternalValue);
        }
    }
}