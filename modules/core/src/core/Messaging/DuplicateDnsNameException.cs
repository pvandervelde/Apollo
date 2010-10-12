//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Apollo.Core.Properties;
using Apollo.Utils;

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// An exception thrown when a duplicate <see cref="DnsName"/> is detected.
    /// </summary>
    [Serializable]
    public sealed class DuplicateDnsNameException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateDnsNameException"/> class.
        /// </summary>
        public DuplicateDnsNameException() 
            : this(Resources_NonTranslatable.Exceptions_Messages_DuplicateDnsName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateDnsNameException"/> class.
        /// </summary>
        /// <param name="name">The <c>DnsName</c> which was a duplicate.</param>
        public DuplicateDnsNameException(DnsName name)
            : this(string.Format(CultureInfo.InvariantCulture, Resources_NonTranslatable.Exceptions_Messages_DuplicateDnsName_WithName, name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateDnsNameException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DuplicateDnsNameException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateDnsNameException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DuplicateDnsNameException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateDnsNameException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private DuplicateDnsNameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
