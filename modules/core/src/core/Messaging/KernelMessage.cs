//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Lokad;

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// Defines the base class for messages send inside the core of the Apollo system.
    /// </summary>
    [Serializable]
    public sealed class KernelMessage : IEquatable<KernelMessage>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(KernelMessage first, KernelMessage second)
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
        public static bool operator !=(KernelMessage first, KernelMessage second)
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
        /// Initializes a new instance of the <see cref="KernelMessage"/> class.
        /// </summary>
        /// <param name="messageToCopy">The message to copy.</param>
        public KernelMessage(KernelMessage messageToCopy)
        {
            {
                Enforce.Argument(() => messageToCopy);
            }

            Header = new MessageHeader(messageToCopy.Header);
            Body = messageToCopy.Body.Copy();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelMessage"/> class.
        /// </summary>
        /// <param name="messageHeader">The header which identifies the message.</param>
        /// <param name="messageBody">The data for the message.</param>
        public KernelMessage(MessageHeader messageHeader, MessageBody messageBody)
        {
            {
                Enforce.Argument(() => messageHeader);
                Enforce.Argument(() => messageBody);
            }

            Header = messageHeader;
            Body = messageBody;
        }

        /// <summary>
        /// Gets the header which identifies this message.
        /// </summary>
        /// <value>The header of the message.</value>
        public MessageHeader Header
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the data for the message.
        /// </summary>
        /// <value>The data that comes with the message.</value>
        public MessageBody Body
        {
            get;
            private set;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="KernelMessage"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(KernelMessage other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other != null)
            {
                return other.Header.Equals(Header);
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
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
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
            return Header.GetHashCode();
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
            return string.Format(
                CultureInfo.InvariantCulture,
                @"{0}" + Environment.NewLine + "with information: {1}", 
                Header, 
                Body);
        }
    }
}
