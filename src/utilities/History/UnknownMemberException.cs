﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Apollo.Utilities.Properties;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// An exception thrown when a history enabled object is created with a member object for which 
    /// no field is defined.
    /// </summary>
    [Serializable]
    public sealed class UnknownMemberException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownMemberException"/> class.
        /// </summary>
        public UnknownMemberException()
            : this(Resources.Exceptions_Messages_UnknownMember)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownMemberException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnknownMemberException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownMemberException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnknownMemberException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownMemberException"/> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information
        ///     about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private UnknownMemberException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
